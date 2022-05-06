using JSONStash.Common.Context;
using JSONStash.Common.Models;
using JSONStash.Web.Service.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JSONStash.Web.Service.Services
{
    public interface IAuthenticateService
    {
        AuthenticateResponse Authenticate(AuthenticateRequest model);
    }

    public class AuthenticateService : IAuthenticateService
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly JSONStashContext _context;
        private readonly IUnlockTokenService _unlockTokenService;

        public AuthenticateService(JSONStashContext context, IConfiguration configuration, ILogger<AuthenticateService> logger, IUnlockTokenService unlockTokenService)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
            _unlockTokenService = unlockTokenService;
        }

        public AuthenticateResponse Authenticate(AuthenticateRequest authenticateRequest)
        {
            User user = _context.Users.FirstOrDefault( user => user.Email.ToLower().Equals(authenticateRequest.Email.ToLower()));

            if (user != null)
            {
                int.TryParse(_configuration["LoginAttemptThreshold"], out int loginAttemptThreshold);

                if (!user.Lock(loginAttemptThreshold))
                {
                    if (user.CheckPassword(authenticateRequest.Password))
                    {
                        JwtSecurityTokenHandler handler = new();

                        byte[] key = Encoding.ASCII.GetBytes(_configuration["JWTSecret"]);

                        int.TryParse(_configuration["JWTExpiresIn"], out int jwtExpiresIn);

                        SecurityTokenDescriptor tokenDescriptor = new()
                        {
                            Subject = new ClaimsIdentity(new[] { new Claim("id", user.UserGuid.ToString()) }),
                            Expires = jwtExpiresIn is not 0
                                ? DateTime.UtcNow.AddMinutes(jwtExpiresIn)
                                : DateTime.UtcNow.AddYears(1),
                            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                        };

                        SecurityToken security = handler.CreateToken(tokenDescriptor);

                        string token = handler.WriteToken(security);

                        DateTimeOffset? expires = (DateTimeOffset?)tokenDescriptor.Expires;

                        _logger.LogInformation($"Successful login by user id: {user.UserGuid}.");

                        return new AuthenticateResponse(user, token, expires, "");
                    }
                    else
                    {
                        user.IncreaseLoginAttempt();

                        _context.SaveChanges();

                        _logger.LogInformation($"User provided a bad password and increased their login attempts.");

                        return new AuthenticateResponse(null, null, null, "Your email or password was not inputted correctly. Please try again.");
                    }
                }
                else
                {
                    _logger.LogInformation($"User was locked out due to reaching more than {loginAttemptThreshold} failed logins.");

                    _context.SaveChanges();

                    bool sent = _unlockTokenService.SendToken(user);

                    return sent ? new AuthenticateResponse(null, null, null, "User has been locked. You will receive an email on how you can unlock your user.") 
                        : new AuthenticateResponse(null, null, null, "There was an issue sending your unlock token. Please, contact the administrator.");
                }
            }
            else
            {
                _logger.LogInformation($"Attempted to find {authenticateRequest.Email} and was unsuccessful.");

                return new AuthenticateResponse(null, null, null, "The email is not registered with this site.");
            }
        }
    }
}

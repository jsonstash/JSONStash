using JSONStash.Common.Context;
using JSONStash.Web.Service.Models;
using JSONStash.Web.Service.Services;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace JSONStash.Web.Service.Middleware
{
    public class JWTAuthenticationMiddleware
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly RequestDelegate _next;

        public JWTAuthenticationMiddleware(RequestDelegate next, IConfiguration configuration, ILogger<JWTAuthenticationMiddleware> logger)
        {
            _next = next;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context, JSONStashContext dbcontext)
        {
            string token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
            {
                try
                {
                    JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                    
                    byte[] key = Encoding.ASCII.GetBytes(_configuration["JWTSecret"]);
                    
                    handler.ValidateToken(token, new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        // Set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later).
                        ClockSkew = TimeSpan.Zero
                    }, out SecurityToken validatedToken);

                    
                    JwtSecurityToken security = (JwtSecurityToken)validatedToken;
                    
                    Guid accountGuid = Guid.Parse(security.Claims.First(x => x.Type == "id").Value);

                    // Attach user to context on successful jwt validation.
                    context.Items["User"] = dbcontext.Users.FirstOrDefault(account => account.UserGuid == accountGuid);
                }
                catch
                {
                    // Do nothing if jwt validation fails.
                    // User is not attached to context so request won't have access to secure routes.
                }
            }

            await _next(context);
        }
    }
}

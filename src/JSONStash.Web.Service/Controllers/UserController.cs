using JSONStash.Common.Context;
using JSONStash.Common.Models;
using JSONStash.Web.Service.Models;
using JSONStash.Web.Service.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.Sqlite;
using JSONStash.Web.Service.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

namespace JSONStash.Web.Service.Controllers
{
    [Route("u")]
    [ApiController]
    [ApiVersion("1.0")]
    public class UserController : Controller
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly JSONStashContext _context;
        private readonly IAuthenticateService _authenticateService;

        public UserController(JSONStashContext context, IConfiguration configuration, ILogger<UserController> logger, IAuthenticateService authenticateService)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
            _authenticateService = authenticateService;
        }

        /// <summary>
        /// Authenticate to api.
        /// </summary>
        /// <param name="authenticateRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("auth")]
        public IActionResult Authenticate()
        {
            bool hasEmail = HttpContext.Request.Headers.TryGetValue("x-auth-email", out StringValues email);
            bool hasPassword = HttpContext.Request.Headers.TryGetValue("x-auth-password", out StringValues password);

            if (hasEmail && hasPassword)
            {
                AuthenticateRequest authenticateRequest = new() { Email = email, Password = password };

                AuthenticateResponse response = _authenticateService.Authenticate(authenticateRequest);

                return Ok(response);
            }
            else
            {
                return BadRequest(new AuthenticateResponse(null, null, "Missing email or password. Please, refer to api documentation for user authentication."));
            }
        }

        /// <summary>
        /// Create user for api.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateAsync()
        {
            try
            {
                bool hasEmail = HttpContext.Request.Headers.TryGetValue("x-auth-email", out StringValues email);
                bool hasPassword = HttpContext.Request.Headers.TryGetValue("x-auth-password", out StringValues password);

                bool exists = await _context.Users.AnyAsync(user => user.Email.ToLower().Equals(email.ToString().ToLower()));

                if (!exists)
                {
                    Guid salt = Guid.NewGuid();

                    using SHA512 sha512Hash = SHA512.Create();

                    byte[] bytes = sha512Hash.ComputeHash(Encoding.UTF8.GetBytes(password.ToString() + salt));

                    User user = new()
                    {
                        Email = email.ToString(),
                        Password = bytes,
                        Salt = salt,
                        Created = DateTimeOffset.UtcNow,
                        Active = true,
                        UserGuid = Guid.NewGuid()
                    };

                    await _context.Users.AddAsync(user);

                    await _context.SaveChangesAsync();

                    return Ok("User was created.");
                }
                else
                {
                    return Conflict("Email already exists.");
                }
            }
            catch
            {
                return BadRequest("There was an issue with your request. Please, contact the administrator.");
            }
        }

        /// <summary>
        /// Unlock user due to lockout.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("unlock")]
        public async Task<IActionResult> UnlockAsync()
        {
            try
            {
                bool hasToken = HttpContext.Request.Headers.TryGetValue("x-unlock-token", out StringValues token);

                if (hasToken)
                {
                    User user = await _context.Users.FirstOrDefaultAsync(acct => acct.ResetToken.Equals(token));

                    user.Unlock(token);

                    await _context.SaveChangesAsync();

                    return Ok("User has been unlocked.");
                }
                else
                {
                    return BadRequest("Could not unlock user due to bad token. Please, contact the administrator.");
                }
            }
            catch
            {
                return BadRequest("There was an issue with your request. Please, contact the administrator.");
            }
        }

        /// <summary>
        /// Resends a new unlock token to user email used for unlocking a user.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("unlock/resend")]
        public async Task<IActionResult> ResendUnlockTokenAsync()
        {
            try
            {
                bool hasUserEmail = HttpContext.Request.Headers.TryGetValue("x-user-email", out StringValues email);
                
                int.TryParse(_configuration["LoginAttemptThreshold"], out int loginAttemptThreshold);

                if (hasUserEmail)
                {
                    User user = await _context.Users.FirstOrDefaultAsync(acct => acct.Email.ToLower().Equals(email.ToString().ToLower()));

                    user.Lock(loginAttemptThreshold);

                    await _context.SaveChangesAsync();

                    string newToken = user.ResetToken;

                    // TODO: Send unlock token email to user.

                    return Ok("A new unlock token has been sent.");
                }
                else
                {
                    return BadRequest("Missing email. Please, refer to api documentation for sending a new unlock token.");
                }
            }
            catch
            {
                return BadRequest("There was an issue with your request. Please, contact the administrator.");
            }
        }

        /// <summary>
        /// Delete user and their stashs.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> DeleteAsync()
        {
            try
            {
                bool hasEmail = HttpContext.Request.Headers.TryGetValue("x-validate-email", out StringValues email);

                User user = (User)HttpContext.Items["User"];

                if (hasEmail)
                    if (email[0].ToLower().Equals(user.Email.ToLower()))
                    {
                        foreach (Collection collection in user.Collections)
                        {
                            foreach (Stash stash in collection.Stashes)
                                _context.Records.RemoveRange(stash.Records);

                            _context.Stashes.RemoveRange(collection.Stashes);
                        }

                        _context.Collections.RemoveRange(user.Collections);

                        _context.Users.Remove(user);

                        await _context.SaveChangesAsync();

                        return Ok("User was deleted.");
                    }

                return BadRequest("Bad email. Please, refer to the api documentation on deleting a user.");
            }
            catch
            {
                return BadRequest("There was an issue with your request. Please, contact the administrator.");
            }
        }
    }
}

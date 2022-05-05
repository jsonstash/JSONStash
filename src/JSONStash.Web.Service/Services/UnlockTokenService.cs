using JSONStash.Common.Context;
using JSONStash.Common.Models;

namespace JSONStash.Web.Service.Services
{
    public interface IUnlockTokenService
    {
        Task<bool> SendToken(User user);
    }

    public class UnlockTokenService : IUnlockTokenService
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly JSONStashContext _context;

        public UnlockTokenService(JSONStashContext context, IConfiguration configuration, ILogger<AuthenticateService> logger)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
        }

        public async Task<bool> SendToken(User user)
        {
            throw new NotImplementedException();
        }
    }
}

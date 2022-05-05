using JSONStash.Common.Context;
using JSONStash.Common.Models;
using System.Net;
using System.Net.Mail;

namespace JSONStash.Web.Service.Services
{
    public interface IUnlockTokenService
    {
        bool SendToken(User user);
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

        public bool SendToken(User user)
        {
            try
            {
                string to = user.Email;

                string host = _configuration["EmailHost"];

                int.TryParse(_configuration["EmailPort"], out int port);

                string username = _configuration["EmailUsername"];

                string password = _configuration["EmailPassword"];

                using SmtpClient client = new(host);

                client.UseDefaultCredentials = false;
                client.Port = port;
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(username, password);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;

                MailMessage mail = new();

                mail.From = new(username);
                mail.To.Add(to);
                mail.Subject = $"JSONStash: User Unlock Token";
                mail.Body = $"Unlock Token: {user.ResetToken}";
                mail.IsBodyHtml = false;

                client.Send(mail);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}

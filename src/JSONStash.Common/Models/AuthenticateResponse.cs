using JSONStash.Common.Models;

namespace JSONStash.Common.Models
{
    public class AuthenticateResponse
    {
        public Guid? Id { get; set; }

        public string Email { get; set; }

        public string Token { get; set; }

        public DateTimeOffset? Expires { get; set; }

        public string Message { get; set; }


        public AuthenticateResponse(User user, string token, DateTimeOffset? expires, string message)
        {
            if (user != null && !string.IsNullOrEmpty(token) && expires != null)
            {
                Id = user.UserGuid;
                Email = user.Email;
                Token = token;
                Expires = expires;
            }
            Message = message;
        }
    }
}

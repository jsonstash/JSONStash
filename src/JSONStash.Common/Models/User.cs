using JSONStash.Common.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text;

namespace JSONStash.Common.Models
{
    public partial class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public byte[] Password { get; set; }

        public RoleType Role { get; set; }

        public Guid Salt { get; set; }

        public bool? IsLocked { get; set; }

        public int LoginAttempts { get; set; } = 0;

        public string ResetToken { get; set; } = null;

        public DateTimeOffset Created { get; set; }

        public DateTimeOffset? Modified { get; set; }

        public Guid UserGuid { get; set; }

        public bool Active { get; set; }


        public virtual ICollection<Collection> Collections { get; set; }
    }

    public partial class User
    {
        public void SetPassword(string password)
        {
            using SHA512 sha512Hash = SHA512.Create();

            Salt = Guid.NewGuid();

            Password = sha512Hash.ComputeHash(Encoding.UTF8.GetBytes(password + Salt));

            Modified = DateTimeOffset.UtcNow;
        }

        public bool CheckPassword(string password)
        {
            if (Password != null)
            {
                using SHA512 sha512Hash = SHA512.Create();

                byte[] bytes = sha512Hash.ComputeHash(Encoding.UTF8.GetBytes(password + Salt));

                return bytes.SequenceEqual(Password);
            }
            else
            {
                return false;
            }
        }

        public bool Lock(int threshold)
        {
            if (LoginAttempts >= threshold)
            {
                IsLocked = true;
                
                ResetToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Replace("==", "");
                
                Modified = DateTimeOffset.UtcNow;

                return true;
            }
            else
            {
                return false;
            }
        }

        public void IncreaseLoginAttempt()
        {
            LoginAttempts++;
            
            Modified = DateTimeOffset.UtcNow;
        }

        public bool Unlock(string token)
        {
            if (ResetToken.Equals(token))
            {
                LoginAttempts = 0;

                IsLocked = false;
                
                ResetToken = null;
                
                Modified = DateTimeOffset.UtcNow;

                return true;
            }
            else
            {
                return false;
            }
        }

        public void ChangePassword(string password)
        {
            SetPassword(password);

            LoginAttempts = 0;

            IsLocked = false;
            
            ResetToken = null;
            
            Modified = DateTimeOffset.UtcNow;
        }
    }
}

using JSONStash.Common.Context;
using JSONStash.Common.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace JSONStash.Common.Tests.Models
{
    [TestClass]
    public class AccountTest
    {
        public readonly User _account;

        public AccountTest()
        {
            _account = new()
            {
                Email = "user@example.com",
                IsLocked = false,
                LoginAttempts = 0,
                ResetToken = null,
                Created = DateTimeOffset.UtcNow,
                Modified = null,
                Active = true,
                UserGuid = Guid.NewGuid()
            };
        }

        [TestMethod]
        public async Task CanBeCreatedAsync()
        {
            using SqliteConnection connection = new("DataSource=:memory:");

            connection.Open();

            DbContextOptions<JSONStashContext> options = new DbContextOptionsBuilder<JSONStashContext>().UseSqlite(connection).Options;

            using JSONStashContext context = new(options);

            await context.Database.EnsureCreatedAsync();

            await context.Users.AddAsync(_account);

            await context.SaveChangesAsync();
        }


        [TestMethod]
        public void CanSetPassword()
        {
            _account.SetPassword("Password,1");

            Assert.IsTrue(_account.Salt != Guid.Empty);

            Assert.IsTrue(_account.Password.Length != 0);
        }

        [TestMethod]
        public void CanCheckPassword()
        {
            string password = "Password,1";

            Assert.IsTrue(!_account.CheckPassword(password));

            _account.SetPassword(password);

            Assert.IsTrue(_account.CheckPassword(password));
        }

        [TestMethod]
        public void CanLock()
        {
            int threshold = 4;

            Assert.IsTrue(!_account.Lock(threshold));

            _account.LoginAttempts = threshold;

            Assert.IsTrue(_account.Lock(threshold));
        }

        [TestMethod]
        public void CanIncreaseLoginAttempt()
        {
            _account.LoginAttempts = 0;

            _account.IncreaseLoginAttempt();

            Assert.IsTrue(_account.LoginAttempts == 1);
        }

        [TestMethod]
        public void CanUnlock()
        {
            string wrongToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Replace("==", "");

            int threshold = 4;

            _account.LoginAttempts = threshold;

            Assert.IsTrue(_account.Lock(threshold));

            Assert.IsTrue(_account.IsLocked);

            Assert.IsTrue(!_account.Unlock(wrongToken));

            Assert.IsTrue(_account.Unlock(_account.ResetToken));

            Assert.IsTrue(!_account.IsLocked);
        }

        [TestMethod]
        public void CanChangePassword()
        {
            string password = "Password,1";

            string newPassword = "Password,2";

            _account.SetPassword(password);

            Assert.IsTrue(_account.CheckPassword(password));

            Assert.IsTrue(!_account.CheckPassword(newPassword));

            _account.ChangePassword(newPassword);

            Assert.IsTrue(!_account.CheckPassword(password));

            Assert.IsTrue(_account.CheckPassword(newPassword));

            Assert.IsTrue(_account.LoginAttempts == 0);

            Assert.IsTrue(!_account.IsLocked);

            Assert.IsTrue(string.IsNullOrEmpty(_account.ResetToken));
        }
    }
}

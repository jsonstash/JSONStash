using JSONStash.Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Security.Cryptography;
using System.Text;

namespace JSONStash.Common.Context
{
    public class JSONStashContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Stash> Stashes { get; set; }
        public virtual DbSet<Record> Records { get; set; }
        public virtual DbSet<Collection> Collections { get; set; }

        protected JSONStashContext()
        {
        }

        public JSONStashContext(DbContextOptions<JSONStashContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            foreach (IMutableEntityType entityType in builder.Model.GetEntityTypes())
                builder.Entity(entityType.ClrType).ToTable(entityType.ClrType.Name);

            builder.Entity<User>(entity =>
            {
                Guid salt = Guid.NewGuid();

                using SHA512 sha512Hash = SHA512.Create();

                entity.HasIndex(e => new { e.Email }).IsUnique();
            });
        }
    }
}

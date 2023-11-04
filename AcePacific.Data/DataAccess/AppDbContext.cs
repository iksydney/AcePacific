using AcePacific.Data.Entities;
using AcePacific.Data.RepositoryPattern.Implementations;
using Microsoft.EntityFrameworkCore;

namespace AcePacific.Data.DataAccess
{
    public class AppDbContext : EntityFrameworkDataContext<AppDbContext>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(c => c.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Cascade;
            }
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<OtpStore> OTPStores { get; set; }
    }
}

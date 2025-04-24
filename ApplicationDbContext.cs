using Aaditya.Models;
using Microsoft.EntityFrameworkCore;

namespace Aaditya.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Login> Login { get; set; }
        public DbSet<Register> Register { get; set; }
        public DbSet<Seller> Seller { get; set; }
        public DbSet<Choco> Chocolate { get; set; }
        public DbSet<User> User { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Login>().ToTable("Login");
            modelBuilder.Entity<Register>().ToTable("Register");
        }
    }
}
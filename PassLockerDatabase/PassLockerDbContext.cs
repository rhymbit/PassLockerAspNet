using System;
using Microsoft.EntityFrameworkCore;

namespace PassLocker.Database
{
    public class PassLockerDbContext : DbContext
    {
        public PassLockerDbContext() { }
        public PassLockerDbContext(DbContextOptions<PassLockerDbContext> options)
                : base(options)
        {          
        }
        public DbSet<User> Users { get; set; }
        public DbSet<UserPasswords> UserPasswords { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(@"Server=PRATEEKPC\SQLEXPRESS;Database=PassLocker;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(user => user.UserName)
                .IsRequired()
                .HasMaxLength(30);

            modelBuilder.Entity<User>()
                .Property(user => user.UserEmail)
                .IsRequired()
                .HasMaxLength(30);

            modelBuilder.Entity<User>()
                .Property(user => user.UserConfirmed)
                .HasDefaultValue(false);

        }
    }
}
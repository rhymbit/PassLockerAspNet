using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace PassLockerDatabase
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
            options.UseSqlServer(
                Environment.GetEnvironmentVariable("DB_SQL_SERVER_EXPRESS") ??
                BackupConnectionString());
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
        
        /// <summary>
        /// This method is for development and testing purpose only.
        /// Remove this method in production
        /// </summary>
        /// <returns>Sql connection string for the current OS.</returns>
        private string BackupConnectionString()
        {
            if (Environment.OSVersion.ToString().Contains("Unix", StringComparison.OrdinalIgnoreCase))
            {
                return "Data Source=accelarator;Initial Catalog=tempdb;User id=sa;Password=Prateek333#;";
            }

            return @"Server=PRATEEKPC\SQLEXPRESS;Database=PassLocker;Trusted_Connection=True;";
        }
    }
}
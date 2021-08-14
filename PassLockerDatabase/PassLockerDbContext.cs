using System;
using Microsoft.EntityFrameworkCore;

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
        public DbSet<UserPassword> UserPasswords { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(
                Environment.GetEnvironmentVariable("DB_SQL_SERVER_EXPRESS") ??
                BackupConnectionString());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // For User model

            modelBuilder.Entity<User>()
                .HasIndex(user => user.UserEmail);
            
            modelBuilder.Entity<User>()
                .Property(user => user.UserName)
                .IsRequired()
                .HasMaxLength(30);

            modelBuilder.Entity<User>()
                .Property(user => user.UserEmail)
                .IsRequired()
                .HasMaxLength(20);

            modelBuilder.Entity<User>()
                .Property(user => user.UserPasswordSalt)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(user => user.UserPasswordHash)
                .IsRequired();
            
            modelBuilder.Entity<User>()
                .Property(user => user.UserSecretSalt)
                .IsRequired();
            
            modelBuilder.Entity<User>()
                .Property(user => user.UserSecretHash)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(user => user.UserConfirmed)
                .HasDefaultValue(false);

            modelBuilder.Entity<User>()
                .Property(user => user.Name)
                .IsRequired()
                .HasMaxLength(30);

            modelBuilder.Entity<User>()
                .Property(user => user.Location)
                .IsRequired()
                .HasMaxLength(10);

            modelBuilder.Entity<User>()
                .Property(user => user.Gender)
                .IsRequired()
                .HasMaxLength(6);

            modelBuilder.Entity<User>()
                .Property(user => user.MemberSince)
                .IsRequired()
                .HasDefaultValue(DateTime.Today.ToShortDateString());

            // For Password model

            modelBuilder.Entity<UserPassword>()
                .HasOne(p => p.User)
                .WithMany(u => u.Passwords);

            modelBuilder.Entity<UserPassword>()
                .Property(pass => pass.DomainName)
                .IsRequired()
                .HasMaxLength(20);

            modelBuilder.Entity<UserPassword>()
                .Property(pass => pass.PasswordSalt)
                .IsRequired();

            modelBuilder.Entity<UserPassword>()
                .Property(pass => pass.PasswordHash)
                .IsRequired();

            modelBuilder.Entity<UserPassword>()
                .Property(pass => pass.UserId)
                .IsRequired();

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
                return "Data Source=acc;Initial Catalog=PassLocker;User id=sa;Password=Prateek332@#;";
            }

            return @"Server=PRATEEKPC\SQLEXPRESS;Database=PassLocker;Trusted_Connection=True;";
        }
    }
}
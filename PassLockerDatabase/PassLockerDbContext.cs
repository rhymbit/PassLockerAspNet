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
                Environment.GetEnvironmentVariable("DB_SQL_SERVER_EXPRESS") ?? string.Empty);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // For User model

            modelBuilder.Entity<User>()
                .HasKey(user => user.UserId);

            modelBuilder.Entity<User>()
                .HasIndex(user => user.UserEmail);

            modelBuilder.Entity<User>()
                .Property(user => user.UserId)
                .IsRequired()
                .HasMaxLength(40)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<User>()
                .Property(user => user.Username)
                .IsRequired()
                .HasMaxLength(30);

            modelBuilder.Entity<User>()
                .Property(user => user.UserEmail)
                .IsRequired()
                .HasMaxLength(50);

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
                .HasMaxLength(40);

            modelBuilder.Entity<User>()
                .Property(user => user.Location)
                .IsRequired()
                .HasMaxLength(20);

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
                .HasKey(pass => pass.UserPasswordId);

            modelBuilder.Entity<UserPassword>()
                .Property(pass => pass.UserPasswordId)
                .IsRequired()
                .HasMaxLength(40)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<UserPassword>()
                .Property(pass => pass.DomainName)
                .IsRequired()
                .HasMaxLength(30);

            modelBuilder.Entity<UserPassword>()
                .Property(pass => pass.PasswordSalt)
                .IsRequired();

            modelBuilder.Entity<UserPassword>()
                .Property(pass => pass.PasswordHash)
                .IsRequired();

            modelBuilder.Entity<UserPassword>()
                .Property(pass => pass.UserId)
                .IsRequired();
            
            // Relationships

            modelBuilder.Entity<UserPassword>()
                .HasOne(p => p.User)
                .WithMany(u => u.Passwords)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
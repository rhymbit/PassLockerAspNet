using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace PassLockerDatabase
{
    [Table("user_passwords")]
    public class UserPassword
    {
        [Column("domain_name")]
        [MaxLength(200)]
        public string DomainName { get; set; }
        
        [Column("password_salt")]
        public string PasswordSalt { get; set; }

        [Column("password_hash")]
        [MaxLength(40)]
        public string PasswordHash { get; set; }
        
        [Column("user_id")]
        public BigInteger UserId { get; set; }

        // Relationship
        [ForeignKey(nameof(UserId))]
        [InverseProperty("StoredPasswords")]
        public virtual User User { get; set; }
    }
}

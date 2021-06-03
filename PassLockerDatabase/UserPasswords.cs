using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PassLocker.Database
{
    [Table("user_passwords")]
    public class UserPasswords
    {
        [Column("user_password_id")]
        public int UserPasswordsId { get; set; }

        [Column("domain_name")]
        [MaxLength(200)]
        public string DomainName { get; set; }


        [Column("domain_password_hash")]
        [MaxLength(40)]
        public string DomainPasswordHash { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        // Relationship
        [ForeignKey(nameof(UserId))]
        [InverseProperty("StoredPasswords")]
        public virtual User User { get; set; }
    }
}

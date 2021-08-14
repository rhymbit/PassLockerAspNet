using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PassLockerDatabase
{
    [Table("user_password")]
    public class UserPassword
    {
        [Column("id")]
        public int UserPasswordId { get; set; }
        
        [Column("domain_name")]
        [MaxLength(200)]
        public string DomainName { get; set; }
        
        [Column("password_salt")]
        public string PasswordSalt { get; set; }

        [Column("password_hash")]
        [MaxLength(40)]
        public string PasswordHash { get; set; }
        
        [Column("user_id")]
        public int UserId { get; set; }
        
        public User User { get; set; }
    }
}

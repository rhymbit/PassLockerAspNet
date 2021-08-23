using System.ComponentModel.DataAnnotations.Schema;

namespace PassLockerDatabase
{
    [Table("user_password")]
    public class UserPassword
    {
        [Column("id")]
        public string UserPasswordId { get; set; }
        
        [Column("domain_name")]
        public string DomainName { get; set; }
        
        [Column("password_salt")]
        public string PasswordSalt { get; set; }

        [Column("password_hash")]
        public string PasswordHash { get; set; }
        
        [Column("user_id")]
        public string UserId { get; set; }
        
        public User User { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace PassLockerDatabase
{
    [Table("users")]
    public class User
    {
        public User()
        {
            StoredPasswords = new HashSet<UserPasswords>();
        }

        [Column("id")]
        public int UserId { get; set; }

        [Column("username")]
        public string UserName { get; set; }

        [Column("email")]
        public string UserEmail { get; set; }

        [NotMapped]
        public string UserPassword { get; set; }

        [Column("password_salt")]
        public string UserPasswordSalt { get; set; }

        [Column("password_hash")]
        public string UserPasswordHash { get; set; }
        
        [Column("secret_salt")]
        public string UserSecretSalt { get; set; }

        [Column("secret_hash")]
        public string UserSecretAnswerHash { get; set; }

        [Column("confirmed")]
        public bool UserConfirmed { get; set; }

        // Some Basic Information
        [Column("name")]
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }

        [Column("location")]
        [Required]
        public string Location { get; set; }

        [Column("gender")]
        [MaxLength(6)]
        [Required]
        public string Gender { get; set; }

        [Column("member_since")]
        public string MemberSince { get; set; }

        // Relationship
        [InverseProperty(nameof(UserPasswords.User))]
        public virtual ICollection<UserPasswords> StoredPasswords { get; set; }
    }
}

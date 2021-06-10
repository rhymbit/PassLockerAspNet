using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace PassLocker.Database
{
    [Table("users")]
    public class User
    {
        public User()
        {
            StoredPasswords = new HashSet<UserPasswords>();
        }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("user_name")]
        public string UserName { get; set; }

        [Column("user_email")]
        public string UserEmail { get; set; }

        [NotMapped]
        public string UserPassword { get; set; }

        [Column("password_salt")]
        public string UserPasswordSalt { get; set; }

        [Column("user_password_hash")]
        public string UserPasswordHash { get; set; }

        [Column("user_secret_answer_hash")]
        public string UserSecretAnswerHash { get; set; }

        [Column("user_confirmed")]
        public bool UserConfirmed { get; set; }

        // Some Basic Information
        [Column("name")]
        [Required]
        [MaxLength(30)]
        public String Name { get; set; }

        [Column("location")]
        [Required]
        public String Location { get; set; }

        [Column("gender")]
        [MaxLength(6)]
        [Required]
        public String Gender { get; set; }

        [Column("member_since")]
        public DateTime MemberSince { get; set; }

        // Relationship
        [InverseProperty(nameof(UserPasswords.User))]
        public virtual ICollection<UserPasswords> StoredPasswords { get; set; }
    }
}

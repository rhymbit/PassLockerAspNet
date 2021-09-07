using System;
using System.Linq;
using System.Threading.Tasks;
using PassLocker.Dto;
using PassLockerDatabase;

namespace PassLocker.Services.UserDatabase
{
    public class UserDatabase : IUserDatabase
    {
        private readonly PassLockerDbContext _db;
        
        public UserDatabase(PassLockerDbContext dbContext)
        {
            _db = dbContext;
        }

        public bool CheckIfUserExists(string email)
        {
            return _db.Users.Any(user => user.UserEmail.Equals(email));
        }

        public UserViewDto GetGoogleUser(string email)
        {
            try
            {
                var googleUser = _db.Users.Single(user => user.UserEmail == email);
                return GoogleUserDto(googleUser);
            }
            catch (ArgumentNullException)
            {
                return NewGoogleUserDto();
            }
            catch (InvalidOperationException)
            {
                return NewGoogleUserDto();
            }
        }
        
        // When user exists in the database
        private static UserViewDto GoogleUserDto(User user) =>
            new()
            {
                UserId = user.UserId,
                UserEmail = user.UserEmail,
                Username = user.Username,
                Confirmed = user.UserConfirmed,
                Name = user.Name,
                Gender = user.Gender,
                Location =  user.Location,
                MemberSince = user.MemberSince
            };
        
        // When user doesn't exists in the database, an empty user is sent to frontend
        // with `UserId = 0`
        private static UserViewDto NewGoogleUserDto() =>
            new()
            {
                UserId = "",
                UserEmail = "",
                Username = "",
                Confirmed = false,
                Name = "",
                Gender = "",
                MemberSince = ""
            };
    }
}
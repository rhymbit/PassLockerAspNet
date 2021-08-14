using System;
using System.Linq;
using System.Threading.Tasks;
using PassLocker.Dto;
using PassLockerDatabase;

namespace PassLocker.Services.UserDatabase
{
    public class UserDatabase : IUserDatabase
    {
        private PassLockerDbContext db;
        
        public UserDatabase(PassLockerDbContext injectContext)
        {
            db = injectContext;
        }

        public async Task<bool> CheckIfUserExists(string email)
        {
            return db.Users.Any(user => user.UserEmail == email);
        }

        public async Task<UserViewDto> GetGoogleUser(string email)
        {
            try
            {
                var googleUser = db.Users.Single(user => user.UserEmail == email);
                return GoogleUserDto(googleUser);
            }
            catch (ArgumentNullException exp)
            {
                return NewGoogleUserDto();
            }
            catch (InvalidOperationException exp)
            {
                return NewGoogleUserDto();
            }
        }
        
        // When user exists in the database
        private static UserViewDto GoogleUserDto(User user) =>
            new UserViewDto()
            {
                UserId = user.UserId,
                UserEmail = user.UserEmail,
                UserName = user.UserName,
                Confirmed = user.UserConfirmed,
                Name = user.Name,
                Gender = user.Gender,
                Location =  user.Location,
                MemberSince = user.MemberSince
            };
        
        // When user doesn't exists in the database, an empty user is sent to frontend
        // with `UserId = 0`
        private static UserViewDto NewGoogleUserDto() =>
            new UserViewDto()
            {
                UserId = "",
                UserEmail = "",
                UserName = "",
                Confirmed = false,
                Name = "",
                Gender = "",
                MemberSince = ""
            };
    }
}
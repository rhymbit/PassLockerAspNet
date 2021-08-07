using System;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<UserViewDTO> GetGoogleUser(string email)
        {
            try
            {
                var googleUser = db.Users.Single(user => user.UserEmail == email);
                return GoogleUserDto(googleUser);
            }
            catch (ArgumentNullException exp)
            {
                return GoogleUserDtoNew();
            }
            catch (InvalidOperationException exp)
            {
                return GoogleUserDtoNew();
            }
        }
        
        // When user exists in the database
        private static UserViewDTO GoogleUserDto(User user) =>
            new UserViewDTO()
            {
                UserId = user.UserId,
                UserEmail = user.UserEmail,
                UserName = user.UserName,
                Confirmed = user.UserConfirmed,
                Name = user.Name,
                Gender = user.Gender,
                MemberSince = user.MemberSince
            };
        
        // When user doesn't exists in the database, an empty user is sent to frontend
        // with `UserId = 0`
        private static UserViewDTO GoogleUserDtoNew() =>
            new UserViewDTO()
            {
                UserId = 0,
                UserEmail = "",
                UserName = "",
                Confirmed = false,
                Name = "",
                Gender = "",
                MemberSince = DateTime.Today
            };
    }
}
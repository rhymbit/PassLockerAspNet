using System;
using System.Linq;
using System.Threading.Tasks;
using PassLocker.Database;

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
    }
}
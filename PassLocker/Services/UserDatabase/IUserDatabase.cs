using System.Threading.Tasks;
using PassLockerDatabase;

namespace PassLocker.Services.UserDatabase
{
    public interface IUserDatabase
    {
        Task<bool> CheckIfUserExists(string email);
        public Task<UserViewDTO> GetGoogleUser(string email);
    }
}
using System.Threading.Tasks;
using PassLocker.Dto;
using PassLockerDatabase;

namespace PassLocker.Services.UserDatabase
{
    public interface IUserDatabase
    {
        Task<bool> CheckIfUserExists(string email);
        public Task<UserViewDto> GetGoogleUser(string email);
    }
}
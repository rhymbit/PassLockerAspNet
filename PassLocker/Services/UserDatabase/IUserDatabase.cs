using System.Threading.Tasks;
using PassLocker.Dto;

namespace PassLocker.Services.UserDatabase
{
    public interface IUserDatabase
    {
        bool CheckIfUserExists(string email);
        public UserViewDto GetGoogleUser(string email);
    }
}
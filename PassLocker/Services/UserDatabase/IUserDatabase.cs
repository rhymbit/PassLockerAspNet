using System.Threading.Tasks;

namespace PassLocker.Services.UserDatabase
{
    public interface IUserDatabase
    {
        Task<bool> CheckIfUserExists(string email);
    }
}
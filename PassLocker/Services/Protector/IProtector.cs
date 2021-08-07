using PassLockerDatabase;

namespace PassLocker.Services.Protector
{
    public interface IProtector
    {
        public User CreateHashedPassword(User user);
        public bool CheckPassword(User user);
    }
}

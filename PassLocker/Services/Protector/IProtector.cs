using PassLockerDatabase;

namespace PassLocker.Services.Protector
{
    public interface IProtector
    {
        public (string, string) CreateHashedStringAndSalt(string stringToHash);
        public bool CheckStringHashing(string hashedString, string salt, string savedHashedString);
    }
}

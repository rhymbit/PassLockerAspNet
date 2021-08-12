using PassLockerDatabase;

namespace PassLocker.Services.Protector
{
    public interface IProtector
    {
        public (string, string) CreateHashedStringAndSalt(string stringToHash);
        public bool VerifyHashing(string providedString, string savedHashedString, string salt);
    }
}

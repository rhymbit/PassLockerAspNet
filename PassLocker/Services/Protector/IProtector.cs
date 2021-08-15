namespace PassLocker.Services.Protector
{
    public interface IProtector
    {
        public string GetUuid();
        public (string, string) CreateHashedStringAndSalt(string stringToHash);
        public bool VerifyHashing(string providedString, string savedHashedString, string salt);
        public string EncryptData(string plainText, string password, string salt);
        public string DecryptData(string cryptoText, string password, string salt);
    }
}

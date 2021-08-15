using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace PassLocker.Services.Protector
{
    public class Protector : IProtector
    {
        public string GetUuid() =>
            Guid.NewGuid().ToString();
        
        // Hashing
        public (string, string) CreateHashedStringAndSalt(string stringToHash)
        {
            // generate random salt
            var saltBytes = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            
            // generate the salted-hashed password
            var saltedHashedString = SaltAndHashString(stringToHash, saltBytes);
            var salt = Convert.ToBase64String(saltBytes);
            return (saltedHashedString, salt);
        }
        
        public bool VerifyHashing(string providedString, string savedHashedString, string salt)
        {
            var saltBytes = Convert.FromBase64String(salt);
            var providedHashedString = SaltAndHashString(providedString, saltBytes);

            return string.Equals(providedHashedString, savedHashedString);
        }

        private static string SaltAndHashString(string password, byte[] salt)
        {
            string saltedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return saltedPassword;
        }
        
        // Encryption-Decryption
        
        private const int Iterations = 2000; // number of iterations for pbkdf to create symmetric key
        public string EncryptData(string plainText, string password, string salt)
        {
            byte[] plainBytes = Encoding.Unicode.GetBytes(plainText);
            byte[] saltBytes = Encoding.Unicode.GetBytes(salt);

            var aes = GetAes(password, saltBytes);
            
            using var ms = new MemoryStream();
            using var cs = new CryptoStream(
                ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
            
            cs.Write(plainBytes);

            var encryptedBytes = ms.ToArray();

            return Convert.ToBase64String(encryptedBytes);
        }

        public string DecryptData(string cryptoText, string password, string salt)
        {
            byte[] cryptoBytes = Convert.FromBase64String(cryptoText);
            byte[] saltBytes = Encoding.Unicode.GetBytes(salt);

            var aes = GetAes(password, saltBytes);
            
            using var ms = new MemoryStream();
            using var cs = new CryptoStream(
                ms, aes.CreateDecryptor(), CryptoStreamMode.Write);
            
            cs.Write(cryptoBytes, 0, cryptoBytes.Length);

            var plainBytes = ms.ToArray();

            return Encoding.Unicode.GetString(plainBytes);
        }

        private static Aes GetAes(string password, byte[] saltBytes)
        {
            var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, Iterations);
            var aes = Aes.Create();
            aes.Key = pbkdf2.GetBytes(32); // setting a 256-bit key
            aes.IV = pbkdf2.GetBytes(16); // setting a 128-bit key
            aes.Padding = PaddingMode.Zeros; // string less than 8 chars won't encoded

            return aes;
        }
    }
}
using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using PassLockerDatabase;

namespace TestingConsole
{
    public class Protector
    {
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

        private static string SaltAndHashString(string stringToHash, byte[] salt)
        {
            string saltedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: stringToHash,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 1000,
                numBytesRequested: 256 / 8));

            return saltedPassword;
        }
    }
}
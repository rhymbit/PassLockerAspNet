using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using PassLocker.Database;

namespace PassLocker.Services.Protector
{
    public class Protector : IProtector
    {
        public User CreateHashedPassword(User user)
        {
            // generate random salt
            var saltBytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }

            // generate the salted-hashed password
            var saltedHashedPassword = SaltAndHashPassword(user.UserPassword, saltBytes);
            user.UserPasswordHash = saltedHashedPassword;
            user.UserPasswordSalt = Convert.ToBase64String(saltBytes);
            return user;
        }

        public bool CheckPassword(User user)
        {
            byte[] saltBytes = Encoding.UTF8.GetBytes(user.UserPasswordSalt);
            // regenerate salted-hashed password

            var saltedHashedPassword = SaltAndHashPassword(user.UserPassword, saltBytes);
            return (saltedHashedPassword == user.UserPasswordHash);
        }

        private static string SaltAndHashPassword(string password, byte[] salt)
        {
            string saltedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return saltedPassword;
        }
    }
}

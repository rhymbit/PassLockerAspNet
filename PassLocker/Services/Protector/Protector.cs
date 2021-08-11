﻿using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using PassLockerDatabase;

namespace PassLocker.Services.Protector
{
    public class Protector : IProtector
    {
        public (string, string) CreateHashedStringAndSalt(string stringToHash)
        {
            // generate random salt
            var saltBytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }

            // generate the salted-hashed password
            var saltedHashedString = SaltAndHashPassword(stringToHash, saltBytes);
            var salt = Convert.ToBase64String(saltBytes);
            return (saltedHashedString, salt);
        }

        public bool CheckHashedString(string hashedString, string salt)
        {
            byte[] saltBytes = Encoding.UTF8.GetBytes(salt);
            // regenerate salted-hashed password

            var saltedHashedString = SaltAndHashPassword(hashedString, saltBytes);
            return (saltedHashedString == hashedString);
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
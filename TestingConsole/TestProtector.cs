using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using PassLockerDatabase;

namespace TestingConsole
{
    
    public class TestProtector
    {
        // encryption-decryption

        private const int Iterations = 2000;

        public string Encrypt(string plainText, string password, string salt)
        {
            byte[] plainBytes = Encoding.Unicode.GetBytes(plainText);
            byte[] saltBytes = Encoding.Unicode.GetBytes(salt);
            

            var aes = Aes.Create(); // Advanced Encryption Standard

            var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, Iterations);

            aes.Key = pbkdf2.GetBytes(32); // setting a 256-bit key
            aes.IV = pbkdf2.GetBytes(16); // setting a 128-bit key
            aes.Padding = PaddingMode.PKCS7;

            using var ms = new MemoryStream();
            using var cs = new CryptoStream(
                ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
            
            cs.Write(plainBytes);

            var encryptedBytes = ms.ToArray();

            return Convert.ToBase64String(encryptedBytes);
        }

        public string Decrypt(string cryptoText, string password, string salt)
        {
            byte[] cryptoBytes = Convert.FromBase64String(cryptoText);
            byte[] saltBytes = Encoding.Unicode.GetBytes(salt);
            
            var aes = Aes.Create(); // Advanced Encryption Standard

            var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, Iterations);

            aes.Key = pbkdf2.GetBytes(32); // setting a 256-bit key
            aes.IV = pbkdf2.GetBytes(16); // setting a 128-bit key
            aes.Padding = PaddingMode.PKCS7;

            using var ms = new MemoryStream();
            using var cs = new CryptoStream(
                ms, aes.CreateDecryptor(), CryptoStreamMode.Write);
            
            cs.Write(cryptoBytes, 0, cryptoBytes.Length);

            var plainBytes = ms.ToArray();

            return Encoding.Unicode.GetString(plainBytes);
        }
        
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
        
        // // encryption-decryption
        //
        // public string EncryptData(string data, string password)
        // {
        //     byte[] byteData = Encoding.Unicode.GetBytes(data);
        //
        //     using MemoryStream ms = new MemoryStream();
        //     
        //     using Aes aes = Aes.Create();
        //     byte[] salt = Encoding.Unicode.GetBytes("BANANAS");
        //     int iterations = 2000;
        //
        //     var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
        //
        //     aes.Key = pbkdf2.GetBytes(32); // set a 256-bit key
        //     aes.IV = pbkdf2.GetBytes(16); // set a 128-bit IV
        //     
        //             
        //     // ms.Write(aes.IV, 0, aes.IV.Length);
        //
        //     using CryptoStream cryptoStream = new CryptoStream(
        //         ms,
        //         aes.CreateEncryptor(),
        //         CryptoStreamMode.Write);
        //    cryptoStream.Write(byteData, 0, byteData.Length);
        //
        //    byte[] encryptedBytes = ms.ToArray();
        //
        //    return Convert.ToBase64String(encryptedBytes);
        // }
        //
        // public string DecryptData(string cryptoData, string password)
        // {
        //     byte[] cryptoBytes = Convert.FromBase64String(cryptoData);
        //     
        //     byte[] salt = Encoding.Unicode.GetBytes("BANANAS");
        //     int iterations = 2000;
        //
        //     var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
        //
        //     var aes = Aes.Create();
        //
        //     aes.Key = pbkdf2.GetBytes(32);
        //     aes.IV = pbkdf2.GetBytes(16);
        //
        //     using MemoryStream ms = new MemoryStream();
        //     using CryptoStream cs = new CryptoStream(
        //         ms, aes.CreateDecryptor(), CryptoStreamMode.Write);
        //     
        //     cs.Write(cryptoBytes, 0, cryptoBytes.Length);
        //     byte[] plainBytes = ms.ToArray();
        //     foreach ( var b in plainBytes)
        //     {
        //         Console.Write(b);
        //     }
        //     Console.WriteLine();
        //
        //     return "ok";
        // }
    }
}
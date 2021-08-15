using System;

namespace TestingConsole
{
    internal class Program
    {
        private static (double, int) GetTuple()
        {
            var tuple = (1.45, 3);
            return tuple;
        }

        private static void Main(string[] args)
        {
            string password = "KhadYH34@#aaA";
            string salt = "YOQAhsIb4$#12#";
            
            var protector = new TestProtector();
            var encryptData = protector.Encrypt("Password",
                password, salt);
            Console.WriteLine(encryptData);
            var decryptData = protector.Decrypt(encryptData,
                password, salt);
            Console.WriteLine(decryptData);
        }
    }
}
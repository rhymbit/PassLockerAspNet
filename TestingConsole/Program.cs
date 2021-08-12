using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using PassLockerDatabase;

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
            const string password = "prateek332";

            var protector = new Protector();

            var (hashedPassword, passwordSalt) = protector.CreateHashedStringAndSalt(password);

            Console.WriteLine(protector.VerifyHashing("prateekasf332", hashedPassword, passwordSalt));
            
            
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using PassLocker.Database;

namespace TestingConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new PassLockerDbContext())
            {
                User firstUser = db.Users.First();
                Console.WriteLine(firstUser.UserEmail);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
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
            Console.WriteLine(DateTime.Today.ToShortDateString());
        }
    }
}
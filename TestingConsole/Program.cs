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
            Guid myuuid = Guid.NewGuid();
            Console.WriteLine(myuuid.GetType());
            string myuuidAsString = myuuid.ToString();
            Console.WriteLine(myuuidAsString.Length);
        }
    }
}
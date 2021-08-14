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
            CheckSqlConnection.Connect();
        }
    }
}
using MySql.Data.MySqlClient;

namespace ThreadedSineCalculator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string connStr = "server=localhost;user=admin;database=sine_database;port=3306;password=admin";
            MySqlConnection conn = new MySqlConnection(connStr);

            ThreadQueueManager.Begin(conn);

            Console.ReadKey();
        }
    }
}
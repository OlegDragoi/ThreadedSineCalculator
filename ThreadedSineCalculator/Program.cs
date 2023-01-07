using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace ThreadedSineCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            string connStr = "server=localhost;user=admin;database=sine_database;port=3306;password=admin";
            MySqlConnection conn = new MySqlConnection(connStr);


            Console.ReadKey();
        }
    }
}

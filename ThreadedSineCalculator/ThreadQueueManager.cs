using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadedSineCalculator
{
    public static class ThreadQueueManager
    {
        static List<KeyValuePair<decimal, ulong>> queue = new List<KeyValuePair<decimal, ulong>>();
        static int maxThreads = Environment.ProcessorCount / 2;
        static MySqlConnection connection;

        public static void Add(decimal x, ulong divisor)
        {
            KeyValuePair<decimal, ulong> newValue = new KeyValuePair<decimal, ulong>(x, divisor);
            queue.Add(newValue);
        }

        public static void Begin(MySqlConnection conn)
        {
            connection = conn;
            ulong currentMaxDivisor = 0;
            try
            {
                conn.Open();

                string sql = "CALL clean()";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
                rdr.Read();
                currentMaxDivisor = (ulong)rdr[0] << 1;

                Console.WriteLine(currentMaxDivisor);

                rdr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            conn.Close();

            try
            {
                conn.Open();

                string sql = "CALL get_initial_values()";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    Add((decimal)rdr[0] - Calculator.PI / currentMaxDivisor, currentMaxDivisor);
                    Add((decimal)rdr[0] + Calculator.PI / currentMaxDivisor, currentMaxDivisor);
                }

                rdr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            conn.Close();
        }

        private static void Process(decimal x, ulong divisor)
        {
            decimal sin_x = Calculator.Sin(x);
            try
            {
                connection.Open();

                string sql = "CALL get_initial_values()";
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                cmd.ExecuteNonQuery();
                divisor <<= 1;
                Add(x - Calculator.PI / divisor, divisor);
                Add(x + Calculator.PI / divisor, divisor);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            connection.Close();
        }

        private static void Queue()
        {
            while (true)
            {
            }
        }
    }
}

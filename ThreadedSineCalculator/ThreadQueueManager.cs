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
        static Dictionary<decimal, ulong> queue = new();
        static int maxThreads = Environment.ProcessorCount;
        static MySqlConnection connection;
        static ulong currentDivisor = 8;
        static ulong currentCount = 0;
        static ulong targetCount;

        public static void Add(decimal x, ulong divisor)
        {
            queue.Add(x, divisor);
        }

        public static void Begin(MySqlConnection conn)
        {
            connection = conn;
            try
            {
                conn.Open();

                string sql = "CALL clean()";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
                rdr.Read();
                currentDivisor = (ulong)rdr[0] << 1;
                targetCount = currentDivisor >> 2;

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
                    Add((decimal)rdr[0] - Calculator.PI / currentDivisor, currentDivisor);
                    Add((decimal)rdr[0] + Calculator.PI / currentDivisor, currentDivisor);
                }

                rdr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            conn.Close();

            Queue();
        }

        private static void Process(decimal x, ulong divisor)
        {
            decimal sin_x = Calculator.Sin(x);
            lock (queue)
            {
                try
                {
                    connection.Open();

                    string sql = String.Format("INSERT INTO sine_database.sine_values (x,sin_x,pi_divisor) VALUES ({0}, {1}, {2})", x, sin_x, divisor);
                    MySqlCommand cmd = new MySqlCommand(sql, connection);
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("{0} -- {1}, {2}", x, sin_x, divisor);
                    divisor <<= 1;
                    Add(x - Calculator.PI / divisor, divisor);
                    Add(x + Calculator.PI / divisor, divisor);
                    ++currentCount;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                connection.Close();
            }
        }

        private static void Queue()
        {
            while (true)
            {
                lock (queue)
                {
                    if (ThreadPool.ThreadCount < maxThreads && queue.ContainsValue(currentDivisor))
                    {
                        KeyValuePair<decimal, ulong> value = queue.First(v => v.Value == currentDivisor);
                        queue.Remove(value.Key);
                        Thread newThread = new Thread(new ThreadStart(() => Process(value.Key, value.Value)));
                        newThread.Start();
                    }
                    if (currentCount == targetCount)
                    {
                        currentCount = 0;
                        targetCount <<= 1;
                        currentDivisor <<= 1;
                    }
                }
            }
        }
    }
}

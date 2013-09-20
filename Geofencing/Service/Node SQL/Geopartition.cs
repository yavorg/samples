using System;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace GeoPrototype
{
    class Program
    {
        /*
        CREATE TABLE GeoFences(fenceId varchar(120) primary key, fence geography, lat decimal(18, 10), long decimal(18, 10));

        CREATE SPATIAL INDEX GeoFences_Location
           ON GeoFences(fence)
           WITH ( GRIDS = ( LEVEL_3 = HIGH, LEVEL_2 = HIGH ) );
        */

        private static string addCommand = @"INSERT INTO GeoFences (fenceId, fence, lat, long)
                                        VALUES ('{0}', geography::Point({1}, {2}, 4326), {1}, {2})";

        private static string queryCommand = @"DECLARE @deviceLocation geography = geography::Point({0}, {1}, 4326);
                                            SELECT top (10) [fenceId], [lat], [long] from GeoFences 
                                            WHERE @deviceLocation.STDistance(fence) < 5000
                                            ORDER BY @deviceLocation.STDistance(fence)";

        static void Main(string[] args)
        {
            try
            {
                SqlConnection sqlConn = new SqlConnection(@"Data Source=tcp:foo.database.windows.net;Initial Catalog=GeoPrototype;Integrated Security=False;User ID=foo;Password=bar;Asynchronous Processing=True;Encrypt=True;TrustServerCertificate=False");
                sqlConn.Open();
                QyeryRow(sqlConn);
                sqlConn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadLine();
        }

        static void QyeryRow(SqlConnection sqlConn)
        {
            string commandToQuery = string.Format(queryCommand, 45.9990335000, -117.6738022000);
            SqlCommand sqlCom = new SqlCommand(commandToQuery, sqlConn);
            SqlDataReader reader = sqlCom.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    Console.WriteLine(String.Format("FenceId = {0}, Lat = {1}, Long = {2}", reader[0], reader[1], reader[2]));
                }
            }
            finally
            {
                reader.Close();
            }
        }

        static void CreateDB(SqlConnection sqlConn)
        {
            int counter = 0;
            string line;

            System.IO.StreamReader file = new System.IO.StreamReader("D:\\Geo\\WA.txt");
            while ((line = file.ReadLine()) != null)
            {
                string[] lineSplits = line.Split('|');
                string fenceId = Regex.Replace(lineSplits[1].Contains("'") ? lineSplits[1].Replace('\'', '_') : lineSplits[1], @"\s+", "");
                string commandToInsert = string.Format(addCommand, lineSplits[0] + "_" + fenceId, lineSplits[9], lineSplits[10]);
                AddRow(commandToInsert, sqlConn);
                counter++;
                Console.Write(".");
            }

            file.Close();
            Console.WriteLine("Done {0}", counter);
        }

        static void AddRow(string command, SqlConnection sqlConn)
        {
            try
            {
                SqlCommand sqlCom = new SqlCommand(command, sqlConn);
                sqlCom.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}

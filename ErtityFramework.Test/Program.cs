using System;
using ErtityFramework.Test.Models;
using ErtityFramework.Tables.MySql;
using ErtityFramework.Database;
using ErtityFramework.Database.MySql;

namespace ErtityFramework.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = @"server=159.89.47.118;user id=remote;password=.Abcd1234!;persistsecurityinfo=True;port=3306;database=ErtisDB;charset=utf8;";
            IDatabaseConnector databaseConnector = new MySqlDatabaseConnector(connectionString);

            databaseConnector.RegisterTableModel<TestModel>();

            int tableNo = 1;
            foreach (var table in databaseConnector.Tables)
            {
                int rowNo = 1;

                Console.WriteLine(string.Format("{0}. {1}", tableNo, table.TableName));
                var rows = table.Select();
                if (rows != null)
                {
                    foreach (var row in rows)
                    {
                        Console.WriteLine(string.Format("\t{0}. {1}", rowNo, row.ToString()));

                        rowNo++;
                    }

                    tableNo++;
                }
            }
        }
    }
}

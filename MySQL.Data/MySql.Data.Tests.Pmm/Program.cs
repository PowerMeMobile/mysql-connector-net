using System;
using System.Threading;
using MySql.Data.MySqlClient;

namespace MySql.Data.Tests.Pmm
{
    class Program
    {
        static int finished = 0;

        static string _server;
        static string _port;
        static string _db;
        static string _user;
        static string _pwd;

        static void Main(string[] args)
        {

            System.Console.WriteLine("Hello World!");

            var c = args.Length == 0 ? 1 : Int32.Parse(args[0]);
            finished = 0;

            _server = args[1];
            _port = args[2];
            _db = args[3];
            _user = args[4];
            _pwd = args[5];

            for (int i = 0; i < c; i++)
            {
                ThreadPool.QueueUserWorkItem(TestDSPerformance2, i + 1);
            }

            while (finished < c)
                Thread.Sleep(2000);

            System.Console.ReadLine();
        }

        private static void TestDSPerformance2(object tharedID)
        {
            string connStr = $"Server={_server};Port={_port};Database={_db};Uid={_user};Pwd={_pwd};Old Guids=true;charset=utf8;Treat Tiny As Boolean=true";

            var conn = new MySqlConnection(connStr);
            conn.Open();
            try
            {
                using (var cmd = new MySqlCommand("op_PreProvisioningItemSelectMigrated", conn, null))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    var adap = new MySqlDataAdapter(cmd);
                    var ds = new System.Data.DataSet();

                    var start = DateTime.Now;

                    adap.Fill(ds);

                    var end = DateTime.Now;
                    var dur = end - start;
                    System.Console.WriteLine($"thread id {tharedID}: {dur}, count: {ds.Tables[0].Rows.Count}");
                }
            }
            finally
            {
                conn.Close();
            }

            finished++;
        }
    }
}

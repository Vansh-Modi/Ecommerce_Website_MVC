using System;
using System.Data.SqlClient;

class Program
{
    static void Main()
    {
        string connStr = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\.Net Projects\MScIT\Ecommerce_Website_MVC\App_Data\ShowsGarageData.mdf;Integrated Security=True";
        using (var conn = new SqlConnection(connStr))
        {
            conn.Open();
            using (var cmd = new SqlCommand("SELECT UserID, FullName, Email, Role, PasswordHash FROM Users", conn))
            using (var r = cmd.ExecuteReader())
            {
                Console.WriteLine("UserID | FullName | Email | Role | PasswordHash");
                while(r.Read())
                {
                    Console.WriteLine(string.Format("{0} | {1} | {2} | {3} | {4}", r[0], r[1], r[2], r[3], r[4]));
                }
            }
        }
    }
}

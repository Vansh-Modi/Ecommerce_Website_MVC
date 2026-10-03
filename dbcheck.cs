using System;
using System.Data.SqlClient;
class Program {
    static void Main() {
        using (var conn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\.Net Projects\MScIT\Ecommerce_Website_MVC\App_Data\ShowsGarageData.mdf;Integrated Security=True")) {
            conn.Open();
            using (var cmd = new SqlCommand("SELECT top 0 * FROM Blogs", conn))
            using (var r = cmd.ExecuteReader()) {
                for (int i = 0; i < r.FieldCount; i++) {
                    Console.WriteLine(r.GetName(i));
                }
            }
        }
    }
}

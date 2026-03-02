using System;
using Microsoft.Data.SqlClient;

class Program
{
    static void Main()
    {
        string connString =
            "Server=localhost;" +
            "Database=EmployeeDB;" +
            "Trusted_Connection=True;" +
            "Encrypt=True;" +
            "TrustServerCertificate=True;";

        using SqlConnection con = new SqlConnection(connString);
        con.Open();
        Console.WriteLine("Connected successfully");
    }
}

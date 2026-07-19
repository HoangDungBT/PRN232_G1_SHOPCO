using System;
using Microsoft.Data.SqlClient;
using BCrypt.Net;

namespace HashTool
{
    class Program
    {
        static void Main(string[] args)
        {
            string plainText = "123456";
            string hash = BCrypt.Net.BCrypt.EnhancedHashPassword(plainText, hashType: HashType.SHA384);
            
            Console.WriteLine($"Hash for 123456: {hash}");

            string connectionString = "Server=localhost;Database=ShopCo;Integrated Security=True;TrustServerCertificate=True;";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                
                // Update ALL users so any account can log in with 123456 for testing
                string query = "UPDATE Users SET PasswordHash = @hash, Status = 'Active' WHERE Status != 'Deleted'";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@hash", hash);
                    int rows = command.ExecuteNonQuery();
                    Console.WriteLine($"{rows} rows updated.");
                }
            }
        }
    }
}

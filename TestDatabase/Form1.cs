using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using MySqlConnector;
using Azure.Identity;
using System.Security.Cryptography;

namespace TestDatabase
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            
        }

        public static string ComputeSHA256(string s)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // Cần chuyển đổi string sang dạng byte khi Hash
                byte[] hashValue = sha256.ComputeHash(Encoding.UTF8.GetBytes(s));
                // Chuyển đổi chuỗi vừa hash sang dạng string để dễ sử dụng
                return BitConverter.ToString(hashValue).Replace("-", "").ToLower();
            }
        }

        private async void connectBtn_Click(object sender, EventArgs e)
        {
            try
            {
                var builder = new MySqlConnectionStringBuilder
                {
                    Server = "databaseforusing.mysql.database.azure.com",
                    Database = "testsql",
                    UserID = "user1admin",
                    Password = "m4tkh4u!123",
                    //SslMode = MySqlSslMode.Required,
                };

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    MessageBox.Show("Đã connect", "Thông báo");
                    await connection.OpenAsync();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "DROP TABLE IF EXISTS UserAdmin;";
                        await command.ExecuteNonQueryAsync();
                        //Console.WriteLine("Finished dropping table (if existed)");

                        command.CommandText = "CREATE TABLE UserAdmin (id serial PRIMARY KEY, user VARCHAR(50), password VARCHAR(50));";
                        await command.ExecuteNonQueryAsync();
                        //Console.WriteLine("Finished creating table");

                        command.CommandText = @"INSERT INTO UserAdmin (user, password) VALUES (@user, @password),
                        (@name2, @quantity2), (@name3, @quantity3);";
                        command.Parameters.AddWithValue("@user1", UserNameTB.Text);
                        command.Parameters.AddWithValue("@password1", ComputeSHA256(PasswordTB.Text));
                        
                        MessageBox.Show("Đã add data", "Thông báo");
                        int rowCount = await command.ExecuteNonQueryAsync();
                        MessageBox.Show(String.Format("Number of rows inserted={0}", rowCount));
                    }
                }
            }
            catch (SqlException xe)
            {
                MessageBox.Show(xe.ToString());
            }
            //
            //Console.ReadLine();
        }

        private async void readBtn_Click(object sender, EventArgs e)
        {
            var builder = new MySqlConnectionStringBuilder
            {
                Server = "databaseforusing.mysql.database.azure.com",
                Database = "testsql",
                UserID = "user1admin",
                Password = "m4tkh4u!123",
                //SslMode = MySqlSslMode.Required,
            };

            using (var conn = new MySqlConnection(builder.ConnectionString))
            {
                MessageBox.Show("Opening connection");
                await conn.OpenAsync();

                using (var command = conn.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM UserAdmin;";
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            MessageBox.Show(string.Format(
                                "Reading from table=({0}, {1}, {2})",
                                reader.GetInt32(0),
                                reader.GetString(1),
                                reader.GetInt32(2)));
                        }
                    }
                    
                }

                MessageBox.Show("Closing connection");
            }
        }

        private async void updateBtn_Click(object sender, EventArgs e)
        {
            var builder = new MySqlConnectionStringBuilder
            {
                Server = "databaseforusing.mysql.database.azure.com",
                Database = "testsql",
                UserID = "user1admin",
                Password = "m4tkh4u!123",
                //SslMode = MySqlSslMode.Required,
            };

            using (var conn = new MySqlConnection(builder.ConnectionString))
            {
                MessageBox.Show("Opening connection");
                await conn.OpenAsync();

                using (var command = conn.CreateCommand())
                {
                    command.CommandText = "UPDATE inventory SET quantity = @quantity WHERE name = @name;";
                    command.Parameters.AddWithValue("@quantity", 200);
                    command.Parameters.AddWithValue("@name", "banana");

                    int rowCount = await command.ExecuteNonQueryAsync();
                    MessageBox.Show(String.Format("Number of rows updated={0}", rowCount));
                }

                MessageBox.Show("Closing connection");
            }
        }
    }
}

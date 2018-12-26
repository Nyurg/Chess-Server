using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using ChessDAL.Models;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;

namespace ChessDAL.Services
{
    public class UserService : IUserService
    {
        string connectionString = ConfigurationManager.ConnectionStrings["ChessDAL.Properties.Settings.ChessDatabaseConnectionString"].ConnectionString.ToString();

        public List<User> RetrieveUsers()
        {
            List<User> users = new List<User>();

            SqlDataReader dataReader;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("SELECT Id, Username, Password " +
                    "FROM Users ORDER BY Id;", connection);

                connection.Open();

                dataReader = command.ExecuteReader();

                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        users.Add(new User
                            (
                                dataReader.GetInt32(0),
                                dataReader.GetString(1),
                                dataReader.GetString(2)
                            ));
                    }
                }

                connection.Close();
            }

            return users;
        }

        public User FindUser(string username, string password)
        {
            User user = null;
            SqlDataReader dataReader;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand($"SELECT Id, Username, Password FROM Users " +
                    $"WHERE Username = '{username}' " +
                    $"AND Password = '{password}'", connection);

                connection.Open();
                dataReader = command.ExecuteReader();

                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        user = new User(dataReader.GetInt32(0),
                            dataReader.GetString(1),
                            dataReader.GetString(2)
                            );
                    }
                }

                connection.Close();
            }

            return user;
        }

    }
}

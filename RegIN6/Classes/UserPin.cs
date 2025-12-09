using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace RegIN6.Classes
{
    using System;
    using MySql.Data.MySqlClient;

    namespace RegIN6.Classes
    {
        public class UserPin
        {
            public int Id { get; set; }
            public int UserId { get; set; }
            public string PinCode { get; set; }
            public bool IsEnabled { get; set; }
            public static UserPin GetUserPin(int userId)
            {
                MySqlConnection connection = WorkingDB.OpenConnection();

                if (!WorkingDB.OpenConnection(connection))
                    return null;

                try
                {
                    string query = "SELECT * FROM user_pins WHERE UserId = @UserId";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", userId);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new UserPin
                                {
                                    Id = reader.GetInt32("Id"),
                                    UserId = reader.GetInt32("UserId"),
                                    PinCode = reader.GetString("PinCode"),
                                    IsEnabled = reader.GetBoolean("IsEnabled")
                                };
                            }
                        }
                    }
                }
                finally
                {
                    WorkingDB.CloseConnection(connection);
                }

                return null;
            }

            public static bool SetUserPin(int userId, string pinCode)
            {
                if (!IsValidPin(pinCode))
                    return false;

                MySqlConnection connection = WorkingDB.OpenConnection();

                if (!WorkingDB.OpenConnection(connection))
                    return false;

                try
                {
                    var existingPin = GetUserPin(userId);

                    if (existingPin != null)
                    {
                        string updateQuery = "UPDATE user_pins SET PinCode = @PinCode WHERE UserId = @UserId";
                        using (MySqlCommand command = new MySqlCommand(updateQuery, connection))
                        {
                            command.Parameters.AddWithValue("@PinCode", pinCode);
                            command.Parameters.AddWithValue("@UserId", userId);
                            command.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        string insertQuery = "INSERT INTO user_pins (UserId, PinCode, IsEnabled) VALUES (@UserId, @PinCode, TRUE)";
                        using (MySqlCommand command = new MySqlCommand(insertQuery, connection))
                        {
                            command.Parameters.AddWithValue("@UserId", userId);
                            command.Parameters.AddWithValue("@PinCode", pinCode);
                            command.ExecuteNonQuery();
                        }
                    }

                    return true;
                }
                finally
                {
                    WorkingDB.CloseConnection(connection);
                }
            }

            public static bool VerifyPin(int userId, string pinCode)
            {
                var userPin = GetUserPin(userId);

                if (userPin == null || !userPin.IsEnabled)
                    return false;

                return userPin.PinCode == pinCode;
            }

            public static bool DeleteUserPin(int userId)
            {
                MySqlConnection connection = WorkingDB.OpenConnection();

                if (!WorkingDB.OpenConnection(connection))
                    return false;

                try
                {
                    string query = "DELETE FROM user_pins WHERE UserId = @UserId";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", userId);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
                finally
                {
                    WorkingDB.CloseConnection(connection);
                }
            }

            private static bool IsValidPin(string pin)
            {
                return !string.IsNullOrEmpty(pin) &&
                       pin.Length == 4 &&
                       int.TryParse(pin, out _);
            }
            public static bool HasPin(int userId)
            {
                return GetUserPin(userId) != null;
            }
        }
    }

}

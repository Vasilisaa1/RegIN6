using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace RegIN6.Classes
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public byte[] Image = new byte[0];
        public string PinCode { get; set; } // Добавили поле для PIN
        public DateTime DateUpdate { get; set; }
        public DateTime DateCreate { get; set; }

        public bool HasPin => !string.IsNullOrEmpty(PinCode); // Свойство для проверки наличия PIN

        public CorrectLogin HandlerCorrectLogin;
        public InCorrectLogin HandlerInCorrectLogin;

        public delegate void CorrectLogin();
        public delegate void InCorrectLogin();

        public void GetUserLogin(string Login)
        {
            this.Id = -1;
            this.Login = String.Empty;
            this.Password = String.Empty;
            this.Name = String.Empty;
            this.Image = new byte[0];
            this.PinCode = String.Empty; // Инициализируем

            MySqlConnection mySqlConnection = WorkingDB.OpenConnection();

            if (WorkingDB.OpenConnection(mySqlConnection))
            {
                MySqlDataReader userQuery = WorkingDB.Query($"SELECT * FROM users WHERE Login = '{Login}'", mySqlConnection);

                if (userQuery.HasRows)
                {
                    userQuery.Read();
                    this.Id = userQuery.GetInt32(0);
                    this.Login = userQuery.GetString(1);
                    this.Password = userQuery.GetString(2);
                    this.Name = userQuery.GetString(3);

                    if (!userQuery.IsDBNull(4))
                    {
                        this.Image = new byte[64 * 1024];
                        userQuery.GetBytes(4, 0, Image, 0, Image.Length);
                    }

                    this.DateUpdate = userQuery.GetDateTime(5);
                    this.DateCreate = userQuery.GetDateTime(6);

                    // Получаем PIN-код, если есть (7-й столбец)
                    if (!userQuery.IsDBNull(7))
                    {
                        this.PinCode = userQuery.GetString(7);
                    }

                    HandlerCorrectLogin.Invoke();
                }
                else
                {
                    HandlerInCorrectLogin.Invoke();
                }
            }
            else
            {
                HandlerInCorrectLogin.Invoke();
            }

            WorkingDB.CloseConnection(mySqlConnection);
        }

        public void SetUser()
        {
            MySqlConnection mySqlConnection = WorkingDB.OpenConnection();

            if (WorkingDB.OpenConnection(mySqlConnection))
            {
                // Обновляем запрос, чтобы включить PinCode
                MySqlCommand mySqlCommand = new MySqlCommand("INSERT INTO users (Login, Password, Name, Image, DateUpdate, DateCreate, PinCode) VALUES (@Login, @Password, @Name, @Image, @DateUpdate, @DateCreate, @PinCode)", mySqlConnection);

                mySqlCommand.Parameters.AddWithValue("@Login", this.Login);
                mySqlCommand.Parameters.AddWithValue("@Password", this.Password);
                mySqlCommand.Parameters.AddWithValue("@Name", this.Name);
                mySqlCommand.Parameters.AddWithValue("@Image", this.Image);
                mySqlCommand.Parameters.AddWithValue("@DateUpdate", this.DateUpdate);
                mySqlCommand.Parameters.AddWithValue("@DateCreate", this.DateCreate);
                mySqlCommand.Parameters.AddWithValue("@PinCode", this.PinCode ?? (object)DBNull.Value);

                mySqlCommand.ExecuteNonQuery();
            }

            WorkingDB.CloseConnection(mySqlConnection);
        }

        // Метод для установки PIN-кода
        public void SetPin(string pin)
        {
            if (!string.IsNullOrEmpty(this.Login) && pin.Length == 4 && int.TryParse(pin, out _))
            {
                MySqlConnection mySqlConnection = WorkingDB.OpenConnection();

                if (WorkingDB.OpenConnection(mySqlConnection))
                {
                    WorkingDB.Query($"UPDATE users SET PinCode='{pin}' WHERE Login = '{this.Login}'", mySqlConnection);
                    this.PinCode = pin; // Обновляем локально
                }

                WorkingDB.CloseConnection(mySqlConnection);
            }
        }

        // Метод для проверки PIN-кода
        public bool CheckPin(string pin)
        {
            return !string.IsNullOrEmpty(this.PinCode) && this.PinCode == pin;
        }

        // Метод для удаления PIN-кода
        public void RemovePin()
        {
            if (!string.IsNullOrEmpty(this.Login))
            {
                MySqlConnection mySqlConnection = WorkingDB.OpenConnection();

                if (WorkingDB.OpenConnection(mySqlConnection))
                {
                    WorkingDB.Query($"UPDATE users SET PinCode=NULL WHERE Login = '{this.Login}'", mySqlConnection);
                    this.PinCode = null; // Обновляем локально
                }

                WorkingDB.CloseConnection(mySqlConnection);
            }
        }

        // Остальные методы без изменений
        public void CrateNewPassword()
        {
            if (Login != String.Empty)
            {
                Password = GeneratePass();

                MySqlConnection mySqlConnection = WorkingDB.OpenConnection();

                if (WorkingDB.OpenConnection(mySqlConnection))
                {
                    WorkingDB.Query($"UPDATE users SET Password='{this.Password}' WHERE Login = '{this.Login}'", mySqlConnection);
                }

                WorkingDB.CloseConnection(mySqlConnection);

                SendMail.SendMessage($"Your account password has been changed.\nNew password: {this.Password}", this.Login);
            }
        }

        public string GeneratePass()
        {
            List<Char> NewPassword = new List<char>();
            Random rnd = new Random();

            char[] ArrNumbers = { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            char[] ArrSymbols = { '-', '_', '!', '@', '#', '$', '%', '^', '&', '*' };
            char[] ArrUppercase = { 'q', 'w', 'e', 'r', 't', 'y', 'u', 'i', 'o', 'p', 'a', 's', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'z', 'x', 'c', 'v', 'b', 'n', 'm' };

            for (int i = 0; i < 1; i++)
            {
                NewPassword.Add(ArrNumbers[rnd.Next(0, ArrNumbers.Length)]);
            }

            for (int i = 0; i < 1; i++)
            {
                NewPassword.Add(ArrSymbols[rnd.Next(0, ArrSymbols.Length)]);
            }

            for (int i = 0; i < 2; i++)
            {
                NewPassword.Add(char.ToUpper(ArrUppercase[rnd.Next(0, ArrUppercase.Length)]));
            }

            for (int i = 0; i < 6; i++)
            {
                NewPassword.Add(ArrUppercase[rnd.Next(0, ArrUppercase.Length)]);
            }

            for (int i = 0; i < NewPassword.Count; i++)
            {
                int RandomSymbol = rnd.Next(0, NewPassword.Count);
                char Symbol = NewPassword[RandomSymbol];
                NewPassword[RandomSymbol] = NewPassword[i];
                NewPassword[i] = Symbol;
            }

            string NPassword = "";

            for (int i = 0; i < NewPassword.Count; i++)
            {
                NPassword += NewPassword[i];
            }

            return NPassword;
        }
    }
}
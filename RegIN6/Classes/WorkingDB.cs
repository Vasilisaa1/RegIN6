using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace RegIN6.Classes
{
    public class WorkingDB
    {
        /// <summary>
        /// Строка подключения к базе данных, указывается сервер, порт подключения, база данных, имя пользователя, пароль пользователя
        /// </summary>
        readonly static string connection = "server=localhost;port=3306;database=regin;user=root;";
        /// <summary>
        /// Создание и открытие подключения
        /// </summary>
        /// <returns>Открытое подключение или null</returns>
        public static MySqlConnection OpenConnection()
        {
            try
            {
                // Создаём подключение MySQL
                MySqlConnection mySqlConnection = new MySqlConnection(connection);
                // Открываем подключение
                mySqlConnection.Open();
                // Возвращаем открытое подключение
                return mySqlConnection;
            }
            catch (Exception exp)
            {
                // В случае ошибки, выводим сообщение в Debug
                Debug.WriteLine(exp.Message);
                // Возвращаем нулевое значение
                return null;
            }
        }

        /// <summary>
        /// Функция выполнения SQL-запросов
        /// </summary>
        /// <param name="Sql">SQL-запрос</param>
        /// <param name="mySqlConnection">Открытое подключение</param>
        /// <returns></returns>
        public static MySqlDataReader Query(string Sql, MySqlConnection mySqlConnection)
        {
            // Создаём команду для выполнения SQL-запроса
            MySqlCommand mySqlCommand = new MySqlCommand(Sql, mySqlConnection);
            // Возвращаем результат выполненной команды
            return mySqlCommand.ExecuteReader();
        }

        /// <summary>
        /// Функция закрытия соединения с базой данных
        /// </summary>
        /// <param name="mySqlConnection">Открытое MySQL соединение</param>
        public static void CloseConnection(MySqlConnection mySqlConnection)
        {
            // Закрываем подключение с базой данных
            mySqlConnection.Close();
            // Очищаем Pool соединения, для того чтобы оно окончательно закрылось, а не ушло в состояние Sleep
            MySqlConnection.ClearPool(mySqlConnection);
        }

        /// <summary>
        /// Функция проверки соединение на работоспособность
        /// </summary>
        /// <param name="mySqlConnection">MySQL соединение</param>
        /// <returns>Статус работоспособности соединения</returns>
        public static bool OpenConnection(MySqlConnection mySqlConnection)
        {
            // Проверяем если соединение не является нулевым
            // И статус соединения Open
            // В таком случае возвращается TRUE, в противоположном FALSE
            return mySqlConnection != null && mySqlConnection.State == System.Data.ConnectionState.Open;
        }

    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RegIN6.Pages
{
    /// <summary>
    /// Логика взаимодействия для Confirmation.xaml
    /// </summary>
    public partial class Confirmation : Page
    {
        public enum TypeConfirmation
        {
            Login,
            Regin
        }

        TypeConfirmation ThisTypeConfirmation;

 
        public int Code = 0;
        public Confirmation(TypeConfirmation typeConfirmation)
        {
            InitializeComponent();

            ThisTypeConfirmation = typeConfirmation;

            SendMailCode();
        }
        public void SendMailCode()
        {
            // Генерируем случайное число
            Code = new Random().Next(100000, 999999);

            // Отправляем число на почту авторизуемого пользователя
            Classes.SendMail.SendMessage($"Login code: {Code}", MainWindow.mainWindow.UserLogIn.Login);

            // Инициализируем процесс в потоке для отправки повторного письма
            Thread TSendMailCode = new Thread(TimerSendMailCode);
            // Отправляем письмо
            TSendMailCode.Start();
        }
        public void TimerSendMailCode()
        {
            // Запускаем цикл в 60 шагов
            for (int i = 0; i < 60; i++)
            {
                // Выполняем вне потока
                Dispatcher.Invoke(() =>
                {
                    // Изменяем данные на текстовом поле
                    LTimer.Content = $"A second message can be sent after {(60 - i)} seconds";
                });
                // Ждём 1 секунду
                Thread.Sleep(1000);
            }
            // По истечению таймера вне потока
            Dispatcher.Invoke(() =>
            {
                // Включаем кнопку отправить повторно
                BSendMessage.IsEnabled = true;
                // Изменяем данные на текстовом поле
                LTimer.Content = "";
            });
        }
        private void SendMail(object sender, RoutedEventArgs e)
        {
            SendMailCode();
        }

        private void OpenLogin(object sender, MouseButtonEventArgs e)
        {
            MainWindow.mainWindow.OpenPage(new Login( ));
        }

        private void SetCode()
        {
           if (TbCode.Text == Code.ToString() && TbCode.IsEnabled == true)
            {
                TbCode.IsEnabled = false;

                if (ThisTypeConfirmation == TypeConfirmation.Login)
                {
                    MessageBox.Show("Авторизация пользователя успешно подтверждена.");

                }

                else
                {
                    MainWindow.mainWindow.UserLogIn.SetUser();
                    MessageBox.Show("Регистрация пользователя успешно подтверждена.");
                }
            }
        }

        private void SetCode(object sender, KeyEventArgs e)
        {
            if (TbCode.Text.Length == 6)
                SetCode();
        }
    }
}

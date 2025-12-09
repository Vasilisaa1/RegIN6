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
            Code = new Random().Next(100000, 999999);
            Classes.SendMail.SendMessage($"Login code: {Code}", MainWindow.mainWindow.UserLogIn.Login);
            Thread TSendMailCode = new Thread(TimerSendMailCode);
            TSendMailCode.Start();
        }
        public void TimerSendMailCode()
        {
            for (int i = 0; i < 60; i++)
            {
                Dispatcher.Invoke(() =>
                {
                    LTimer.Content = $"A second message can be sent after {(60 - i)} seconds";
                });
                Thread.Sleep(1000);
            }
            Dispatcher.Invoke(() =>
            {
                BSendMessage.IsEnabled = true;
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

                    if (MainWindow.mainWindow.UserLogIn.HasPin)
                    {
                        var result = MessageBox.Show("Использовать быстрый вход по PIN-коду в следующий раз?",
                            "Быстрая авторизация", MessageBoxButton.YesNo, MessageBoxImage.Question);

                        if (result == MessageBoxResult.Yes)
                        {
                            MessageBox.Show("PIN-код уже установлен. При следующем входе используйте быстрый вход.",
                                "Информация", MessageBoxButton.OK, MessageBoxImage.Information);

                            // MainWindow.mainWindow.OpenPage(new MainPage());
                        }
                        else
                        {
                            MainWindow.mainWindow.OpenPage(new Login());
                        }
                    }
                    else
                    {
                        var result = MessageBox.Show("Хотите установить 4-значный PIN-код для быстрой авторизации?",
                            "Настройка PIN", MessageBoxButton.YesNo, MessageBoxImage.Question);

                        if (result == MessageBoxResult.Yes)
                        {
                            MainWindow.mainWindow.OpenPage(new PinPage(PinPage.PinMode.Setup));
                        }
                        else
                        {
                            MainWindow.mainWindow.OpenPage(new Login());
                        }
                    }
                }
                else 
                {
                    MainWindow.mainWindow.UserLogIn.SetUser();
                    MessageBox.Show("Регистрация пользователя успешно подтверждена.");

                    var result = MessageBox.Show("Хотите установить 4-значный PIN-код для быстрой авторизации?",
                        "Настройка PIN", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        MainWindow.mainWindow.OpenPage(new PinPage(PinPage.PinMode.Setup));
                    }
                    else
                    {
                        MainWindow.mainWindow.OpenPage(new Login());
                    }
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

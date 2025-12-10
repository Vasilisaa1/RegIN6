using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RegIN6.Classes;

namespace RegIN6.Pages
{
    /// <summary>
    /// Логика взаимодействия для Login.xaml
    /// </summary>
    public partial class Login : Page
    {
        string OldLogin;
        int CountSetPassword = 2;
        bool IsCapture = false;

        public Login()
        {
            InitializeComponent();
            MainWindow.mainWindow.UserLogIn.HandlerCorrectLogin += CorrectLogin;
            MainWindow.mainWindow.UserLogIn.HandlerInCorrectLogin += InCorrectLogin;
            Capture.HandlerCorrectCapture += CorrectCapture;
        }

        public void CorrectLogin()
        {
            if (OldLogin != TbLogin.Text)
            {
                SetNotification("Hi, " + MainWindow.mainWindow.UserLogIn.Name, Brushes.Black);

                try
                {
                    BitmapImage biImg = new BitmapImage();
                    MemoryStream ms = new MemoryStream(MainWindow.mainWindow.UserLogIn.Image);

                    biImg.BeginInit();
                    biImg.StreamSource = ms;
                    biImg.EndInit();

                    ImageSource imgSrc = biImg;

                    DoubleAnimation StartAnimation = new DoubleAnimation();
                    StartAnimation.From = 1;
                    StartAnimation.To = 0;
                    StartAnimation.Duration = TimeSpan.FromSeconds(0.6);
                    StartAnimation.Completed += delegate
                    {
                        IUser.Source = imgSrc;
                        DoubleAnimation EndAnimation = new DoubleAnimation();
                        EndAnimation.From = 0;
                        EndAnimation.To = 1;
                        EndAnimation.Duration = TimeSpan.FromSeconds(1.2);
                        IUser.BeginAnimation(Image.OpacityProperty, EndAnimation);
                    };
                    IUser.BeginAnimation(Image.OpacityProperty, StartAnimation);
                }
                catch (Exception exp)
                {
                    Debug.WriteLine(exp.Message);
                }

                OldLogin = TbLogin.Text;

                // ПОКАЗЫВАЕМ КНОПКУ БЫСТРОГО ВХОДА, ЕСЛИ ЕСТЬ PIN
                if (MainWindow.mainWindow.UserLogIn.HasPin)
                {
                    BtnQuickLogin.Visibility = Visibility.Visible;
                }
                else
                {
                    BtnQuickLogin.Visibility = Visibility.Collapsed;
                }
            }
        }

        public void InCorrectLogin()
        {
            // Если пользователь идентифицирован как личность, или указаны ошибки
            if (LNameUser.Content != "")
            {
                LNameUser.Content = "";

                DoubleAnimation StartAnimation = new DoubleAnimation();

                StartAnimation.From = 1;

                StartAnimation.To = 0;

                StartAnimation.Duration = TimeSpan.FromSeconds(0.6);

                StartAnimation.Completed += delegate
                {
                    IUser.Source = new BitmapImage(new Uri("pack://application:,,,/Images/user.png"));

                    DoubleAnimation EndAnimation = new DoubleAnimation();

                    EndAnimation.From = 0;

                    EndAnimation.To = 1;

                    EndAnimation.Duration = TimeSpan.FromSeconds(1.2);
                    IUser.BeginAnimation(OpacityProperty, EndAnimation);
                };
                IUser.BeginAnimation(OpacityProperty, StartAnimation);
            }
            if (TbLogin.Text.Length > 0)
                SetNotification("Login is incorrect", Brushes.Red);

            // Скрываем кнопку быстрого входа при неверном логине
            BtnQuickLogin.Visibility = Visibility.Collapsed;
        }

        public void CorrectCapture()
        {
            Capture.IsEnabled = false;
            IsCapture = true;
        }

        private void SetPassword(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SetPassword();
            }
        }

        public void SetPassword()
        {
            if (MainWindow.mainWindow.UserLogIn.Password != String.Empty)
            {
                if (IsCapture)
                {
                    if (MainWindow.mainWindow.UserLogIn.Password == TbPassword.Password)
                    {
                        // ПРОВЕРЯЕМ, ЕСТЬ ЛИ У ПОЛЬЗОВАТЕЛЯ PIN
                        if (MainWindow.mainWindow.UserLogIn.HasPin)
                        {
                            // ЕСТЬ PIN - ПРЕДЛАГАЕМ БЫСТРЫЙ ВХОД
                            var result = MessageBox.Show("Использовать быстрый вход по PIN-коду?",
                                "Быстрая авторизация", MessageBoxButton.YesNo, MessageBoxImage.Question);

                            if (result == MessageBoxResult.Yes)
                            {
                                MainWindow.mainWindow.OpenPage(new PinPage(PinPage.PinMode.Login));
                            }
                            else
                            {
                                // ИДЕМ НА ПОДТВЕРЖДЕНИЕ ЧЕРЕЗ ПОЧТУ
                                MainWindow.mainWindow.OpenPage(new Confirmation(Confirmation.TypeConfirmation.Login));
                            }
                        }
                        else
                        {
                            // НЕТ PIN - ИДЕМ НА ПОДТВЕРЖДЕНИЕ ЧЕРЕЗ ПОЧТУ
                            MainWindow.mainWindow.OpenPage(new Confirmation(Confirmation.TypeConfirmation.Login));
                        }
                    }
                    else
                    {
                        if (CountSetPassword > 0)
                        {
                            SetNotification($"Password is incornect, {CountSetPassword} attempts left", Brushes.Red);
                            CountSetPassword--;
                        }
                    }
                }
                else
                {
                    Thread TBlockAutorization = new Thread(BlockAutorization);
                    TBlockAutorization.Start();
                    SendMail.SendMessage("An attempt was made to log into your account.", MainWindow.mainWindow.UserLogIn.Login);
                }
            }
            else
            {
                SetNotification($"Enter capture", Brushes.Red);
            }
        }

        // НОВЫЙ МЕТОД ДЛЯ КНОПКИ БЫСТРОГО ВХОДА
        private void BtnQuickLogin_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.mainWindow.UserLogIn.HasPin)
            {
                MainWindow.mainWindow.OpenPage(new PinPage(PinPage.PinMode.Login));
            }
            else
            {
                MessageBox.Show("У вас не установлен PIN-код. Сначала войдите обычным способом.",
                    "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public void BlockAutorization()
        {
            DateTime StartBlock = DateTime.Now.AddMinutes(3);
            Dispatcher.Invoke(() =>
            {
                TbLogin.IsEnabled = false;
                TbPassword.IsEnabled = false;
                Capture.IsEnabled = false;
                BtnQuickLogin.IsEnabled = false; // Отключаем кнопку тоже
            });

            for (int i = 0; i < 180; i++)
            {
                TimeSpan TimeIdle = StartBlock.Subtract(DateTime.Now);
                string s_minutes = TimeIdle.Minutes.ToString();
                if (TimeIdle.Minutes < 10)
                    s_minutes = "0" + TimeIdle.Minutes;
                string s_seconds = TimeIdle.Seconds.ToString();
                if (TimeIdle.Seconds < 10)
                    s_seconds = "0" + TimeIdle.Seconds;
                Dispatcher.Invoke(() =>
                {
                    SetNotification($"Reauthorization available in: {s_minutes}:{s_seconds}", Brushes.Red);
                });
                Thread.Sleep(1000);
            }
            Dispatcher.Invoke(() =>
            {
                SetNotification("Hi, " + MainWindow.mainWindow.UserLogIn.Name, Brushes.Black);
                TbLogin.IsEnabled = true;
                TbPassword.IsEnabled = true;
                Capture.IsEnabled = true;
                Capture.CreateCapture();
                IsCapture = false;
                CountSetPassword = 2;
                BtnQuickLogin.IsEnabled = true; // Включаем кнопку обратно
            });
        }

        private void SetLogin(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                MainWindow.mainWindow.UserLogIn.GetUserLogin(TbLogin.Text);

                if (TbPassword.Password.Length > 0)
                    SetPassword();
            }
        }

        private void SetLogin(object sender, RoutedEventArgs e)
        {
            MainWindow.mainWindow.UserLogIn.GetUserLogin(TbLogin.Text);

            if (TbPassword.Password.Length > 0)
                SetPassword();
        }

        public void SetNotification(string Message, SolidColorBrush _Color)
        {
            LNameUser.Content = Message;
            LNameUser.Foreground = _Color;
        }

        private void RecoveryPassword(object sender, MouseButtonEventArgs e) =>
            MainWindow.mainWindow.OpenPage(new Recovery());

        private void OpenRegin(object sender, MouseButtonEventArgs e) =>
            MainWindow.mainWindow.OpenPage(new Regin());
    }
}
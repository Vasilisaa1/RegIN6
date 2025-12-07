using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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
                    IUser.Source = new BitmapImage(new Uri("pack://application:,,,/Images/ic-user.png"));

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
            // Если пароль пользователя загруженного из БД не пустой
            // Значит что пользователь ввёл правильный логин
            if (MainWindow.mainWindow.UserLogIn.Password != String.Empty)
            {
                // Если капча пройдена
                if (IsCapture)
                {
                    // Если пароль загруженного пользователя совпадает с паролем введённым в поле
                    if (MainWindow.mainWindow.UserLogIn.Password == TbPassword.Password)
                    {
                        // Перенаправляем пользователя на страницу подтверждения
                        // Сообщаем странице, что проходим подтверждение на авторизацию
                        MainWindow.mainWindow.OpenPage(new Confirmation(Confirmation.TypeConfirmation.Login));
                    }
                    else
                    {
                        // Если пароль не совпадает с загруженным пользователя
                        if (CountSetPassword > 0)
                        {
                            // Выводим предупреждение, сколько попыток осталось, цвет = красный
                            SetNotification($"Password is incornect, {CountSetPassword} attempts left", Brushes.Red);


                            // Вычитаем попытку ввода пароля  
                            CountSetPassword--;
                        }
                    }
                }
                else
                {
                    // Если попытки ввода пароля закончились  
                    // Создаём поток  
                    Thread TBlockAutorization = new Thread(BlockAutorization);
                    // Запускаем поток  
                    TBlockAutorization.Start();

                    // Отправляем сообщение пользователю о том, что под его аккаунтом кто-то пытается авторизоваться  
                    SendMail.SendMessage("An attempt was made to log into your account.", MainWindow.mainWindow.UserLogIn.Login);
                }
                
            }
            else
            {
                // Если капча не пройдена, вызываем ошибку, цвет - красный  
                SetNotification($"Enter capture", Brushes.Red);
            }
        }
        public void BlockAutorization()
        {
            // Запоминаем время блокировки
            DateTime StartBlock = DateTime.Now.AddMinutes(3);
            // Выполняем вне потока
            Dispatcher.Invoke(() =>
            {
                // Отключаем окно ввода логина
                TbLogin.IsEnabled = false;
                // Отключаем окно ввода пароля
                TbPassword.IsEnabled = false;
                // Отключаем окно ввода капчи
                Capture.IsEnabled = false;
            });

            // Запускаем цикл в 180 шагов | 180/60 = 3 минуты
            for (int i = 0; i < 180; i++)
            {
                // получаем оставшееся время
                TimeSpan TimeIdle = StartBlock.Subtract(DateTime.Now);
                // Получаем минуты
                string s_minutes = TimeIdle.Minutes.ToString();
                // Если минуты меньше 10
                if (TimeIdle.Minutes < 10)
                    // Добавляем 0
                    s_minutes = "0" + TimeIdle.Minutes;
                // Получаем секунды
                string s_seconds = TimeIdle.Seconds.ToString();
                // Если секунды меньше 10
                if (TimeIdle.Seconds < 10)
                    // Добавляем 0
                    s_seconds = "0" + TimeIdle.Seconds;
                // Вне потока
                Dispatcher.Invoke(() =>
                {
                    // Выводим время до разблокировки, цвет красный
                    SetNotification($"Reauthorization available in: {s_minutes}:{s_seconds}", Brushes.Red);
                });
                // Ждём 1 секунду
                Thread.Sleep(1000);
            }
            // Вне потока
            Dispatcher.Invoke(() =>
            {
                // Выводим логин авторизованного пользователя, цвет чёрный
                SetNotification("Hi, " + MainWindow.mainWindow.UserLogIn.Name, Brushes.Black);
                // Включаем ввод логина
                TbLogin.IsEnabled = true;
                // Включаем ввод пароля
                TbPassword.IsEnabled = true;
                // Включаем капчу
                Capture.IsEnabled = true;
                // Вызываем генерацию новой капчи
                Capture.CreateCapture();
                // Запоминаем о том что капча не введена
                IsCapture = false;
                // Устанавливаем кол-во попыток 2
                CountSetPassword = 2;
            });
        }
    }
}

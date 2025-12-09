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

namespace RegIN6.Pages
{
    /// <summary>
    /// Логика взаимодействия для Recovery.xaml
    /// </summary>
    public partial class Recovery : Page
    {
        bool IsCapture = false;
        string OldLogin;
        public Recovery()
        {
            InitializeComponent();


            MainWindow.mainWindow.UserLogIn.HandlerCorrectLogin += CorrectLogin;
            MainWindow.mainWindow.UserLogIn.HandlerInCorrectLogin += InCorrectLogin;

            Capture.HandlerCorrectCapture += CorrectCapture;
        }
        private void CorrectLogin()
        {
            // Если предыдущий введённый логин не равен тому что введён сейчас
            if (OldLogin != TbLogin.Text)
            {
                // Вызываем метод уведомления, передавая сообщение, имя пользователя и цвет
                SetNotification("Hi, " + MainWindow.mainWindow.UserLogIn.Name, Brushes.Black);
                // Используем конструкцию try-catch
                try
                {
                    // Инициализируем BitmapImage, который будет содержать изображение пользователя
                    BitmapImage bitImg = new BitmapImage();
                    // Открываем поток, хранилищем которого является память и указываем в качестве источника
                    // массив байт изображения пользователя
                    MemoryStream ms = new MemoryStream(MainWindow.mainWindow.UserLogIn.Image);
                    // Сигнализируем о начале инициализации
                    bitImg.BeginInit();
                    // Указываем источник потока
                    bitImg.StreamSource = ms;
                    // Сигнализируем о конце инициализации
                    bitImg.EndInit();
                    // Получаем ImageSource
                    ImageSource imgSrc = bitImg;
                    // Создаём анимацию старта
                    DoubleAnimation StartAnimation = new DoubleAnimation();
                    // Указываем значение от которого она выполняется
                    StartAnimation.From = 1;
                    // Указываем значение до которого она выполняется
                    StartAnimation.To = 0;
                    // Указываем продолжительность выполнения
                    StartAnimation.Duration = TimeSpan.FromSeconds(0.6);
                    // Присваиваем событие при конце анимации
                    StartAnimation.Completed += delegate
                    {
                        // Устанавливаем изображение
                        IUser.Source = imgSrc;
                        // Создаём анимацию конца
                        DoubleAnimation EndAnimation = new DoubleAnimation();
                        // Указываем значение от которого она выполняется
                        EndAnimation.From = 0;
                        // Указываем значение до которого она выполняется
                        EndAnimation.To = 1;
                        // Указываем продолжительность выполнения
                        EndAnimation.Duration = TimeSpan.FromSeconds(1.2);
                        // Запускаем анимацию плавной смены на изображении
                        IUser.BeginAnimation(Image.OpacityProperty, EndAnimation);
                    };
                    // Запускаем анимацию плавной смены на изображении
                    IUser.BeginAnimation(Image.OpacityProperty, StartAnimation);
                }
                catch (Exception exp)
                {
                    // Если возникла ошибка, выводим в дебаг
                    Debug.WriteLine(exp.Message);
                }
                // Запоминаем введённый логин
                OldLogin = TbLogin.Text;
                // Вызываем метод создания нового пароля
                SendNewPassword();
            }
        }
        public void SetNotification(string Message, SolidColorBrush _Color)
        {
            // Для текстового поля указываем текст
            LNameUser.Content = Message;
            // Для текстового поля указываем цвет
            LNameUser.Foreground = _Color;
        }
        private void InCorrectLogin()
        {
            // Если пользователь идентифицирован как личность, или указаны ошибки
            if (LNameUser.Content != "")
            {
                // Очищаем приветствие пользователя
                LNameUser.Content = "";
                // Создаём анимацию старта
                DoubleAnimation StartAnimation = new DoubleAnimation();
                // Указываем значение от которого она выполняется
                StartAnimation.From = 1;
                // Указываем значение до которого она выполняется
                StartAnimation.To = 0;
                // Указываем продолжительность выполнения
                StartAnimation.Duration = TimeSpan.FromSeconds(0.6);
                // Присваиваем событие при конце анимации
                StartAnimation.Completed += delegate
                {
                    // Указываем стандартный логотип в качестве изображения пользователя
                    IUser.Source = new BitmapImage(new Uri("pack://application:,,,/Images/user.png"));
                    // Создаём анимацию конца
                    DoubleAnimation EndAnimation = new DoubleAnimation();
                    // Указываем значение от которого она выполняется
                    EndAnimation.From = 0;
                    // Указываем значение до которого она выполняется
                    EndAnimation.To = 1;
                    // Указываем продолжительность выполнения
                    EndAnimation.Duration = TimeSpan.FromSeconds(1.2);
                    // Запускаем анимацию плавной смены на изображении
                    IUser.BeginAnimation(OpacityProperty, EndAnimation);
                };
                // Запускаем анимацию плавной смены на изображении
                IUser.BeginAnimation(OpacityProperty, StartAnimation);
            }

            // сообщение о том что логин введён не правильно
            if (TbLogin.Text.Length > 0)
                // Выводим сообщение о том, что логин введён не верно, цвет текста красный
                SetNotification("Login is incorrect", Brushes.Red);
        }
        private void CorrectCapture()
        {
            // Отключаем элемент капчи
            Capture.IsEnabled = false;
            // Запоминаем что ввод капчи осуществлён
            IsCapture = true;
            // Вызываем генерацию нового пароля
            SendNewPassword();
        }
        private void SetLogin(object sender, KeyEventArgs e)
        {
            // Если нажата клавиша Enter
            if (e.Key == Key.Enter)
                // Вызываем получение данных пользователя по логину
                MainWindow.mainWindow.UserLogIn.GetUserLogin(TbLogin.Text);
        }

        /// <summary>
        /// Метод ввода логина
        /// </summary>
        private void SetLogin(object sender, RoutedEventArgs e) =>
            // Вызываем получение данных пользователя по логину
            MainWindow.mainWindow.UserLogIn.GetUserLogin(TbLogin.Text);
        public void SendNewPassword()
        {
            // Если пройдена капча
            if (IsCapture)
            {
                // Если пароль не является пустым, а это значит пользователь ввёл правильную почту
                if (MainWindow.mainWindow.UserLogIn.Password != String.Empty)
                {
                    // Создаём анимацию старта
                    DoubleAnimation StartAnimation = new DoubleAnimation();
                    // Указываем значение от которого она выполняется
                    StartAnimation.From = 1;
                    // Указываем значение до которого она выполняется
                    StartAnimation.To = 0;
                    // Указываем продолжительность выполнения
                    StartAnimation.Duration = TimeSpan.FromSeconds(0.6);
                    // Присваиваем событие при конце анимации
                    StartAnimation.Completed += delegate
                    {
                        // Указываем стандартный логотип в качестве изображения пользователя
                        IUser.Source = new BitmapImage(new Uri("pack://application:,,,/Images/gmail.png"));
                        // Создаём анимацию конца
                        DoubleAnimation EndAnimation = new DoubleAnimation();
                        // Указываем значение от которого она выполняется
                        EndAnimation.From = 0;
                        // Указываем значение до которого она выполняется
                        EndAnimation.To = 1;
                        // Указываем продолжительность выполнения
                        EndAnimation.Duration = TimeSpan.FromSeconds(1.2);
                        // Запускаем анимацию плавной смены на изображении
                        IUser.BeginAnimation(OpacityProperty, EndAnimation);
                    };
                    // Запускаем анимацию плавной смены на изображении
                    IUser.BeginAnimation(OpacityProperty, StartAnimation);
                    // Выводим сообщение о том что новый пароль будет отправлен на почту
                    SetNotification("An email has been sent to your email.", Brushes.Black);
                    // Вызываем функцию создания нового пароля
                    MainWindow.mainWindow.UserLogIn.CrateNewPassword();
                }
            }
        }
        private void OpenLogin(object sender, MouseButtonEventArgs e)
        {
            MainWindow.mainWindow.OpenPage(new Login());
        }
    }
}

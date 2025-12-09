using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using Imaging = Aspose.Imaging;

namespace RegIN6.Pages
{
   
    public partial class Regin : Page
    {
        OpenFileDialog FileDialogImage = new OpenFileDialog();
        bool BCorrectLogin = false;
        bool BCorrectPassword = false;
        bool BcorrectConfirmPassword = false;
        bool BSetImage = false;
        public Regin()
        {
            InitializeComponent();
            MainWindow.mainWindow.UserLogIn.HandlerCorrectLogin += CorrectLogin;
            MainWindow.mainWindow.UserLogIn.HandlerInCorrectLogin += InCorrectLogin;

            FileDialogImage.Filter = "PNG (*.png)|*.png|JPG (*.jpg)|*.jpg";
            FileDialogImage.RestoreDirectory = true;
            FileDialogImage.Title = "Choose a phoro for your avatar";
        }
        private void CorrectLogin()
        {

            SetNotification("Login already in use", Brushes.Red);
            BCorrectLogin = false;
        }

        private void InCorrectLogin() =>
            SetNotification("", Brushes.Black);
        private void SetLogin(object sender, KeyEventArgs e)
        {
          
            if (e.Key == Key.Enter)
                
                SetLogin();
        }
        private void SetLogin(object sender, System.Windows.RoutedEventArgs e) =>
           
            SetLogin();
        public void SetLogin()
        {
           
            Regex regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
         
            BCorrectLogin = regex.IsMatch(TbLogin.Text);
            
            if (regex.IsMatch(TbLogin.Text) == true)
            {
                SetNotification("", Brushes.Black);
                  MainWindow.mainWindow.UserLogIn.GetUserLogin(TbLogin.Text);
            }
            else
            {
                SetNotification("Invalid login", Brushes.Red);
                OnRegin();
            }
        }
        private void SetPassword(object sender, System.Windows.RoutedEventArgs e) =>
  
    SetPassword();

        private void SetPassword(object sender, KeyEventArgs e)
        {
           
            if (e.Key == Key.Enter)
              
                SetPassword();
        }

        public void SetPassword()
        {
            
            Regex regex = new Regex(@"(?=.*[0-9])(?=.*[!@#$%^&*\\-_=])(?=.*[a-z])(?=.*[A-Z])[0-9a-zA-Z!@#$%^&*\\-_=]{10,}");

         
            BCorrectPassword = regex.IsMatch(TbPassword.Password);
        
            if (regex.IsMatch(TbPassword.Password) == true)
            {
               
                SetNotification("", Brushes.Black);
  
                if (tbConfirmPassword.Password.Length > 0)
                
                    ConfirmPassword(true);
           
                OnRegin();
            }
            else
            {
                
                SetNotification("Invalid password", Brushes.Red);
            }
        }
        private void ConfirmPassword(object sender, KeyEventArgs e)
        {
           
            if (e.Key == Key.Enter)
                ConfirmPassword();
        }

       
        private void ConfirmPassword(object sender, System.Windows.RoutedEventArgs e) =>
          
            ConfirmPassword();

 
        public void ConfirmPassword(bool Pass = false)
        {

            BcorrectConfirmPassword = tbConfirmPassword.Password == TbPassword.Password;
  
            if (tbConfirmPassword.Password != TbPassword.Password)
             
                SetNotification("Passwords do not match", Brushes.Red);
            else
            {
              
                SetNotification("", Brushes.Black);
                if (!Pass)
                    SetPassword();
            }
        }
        void OnRegin()
        {
            
            if (!BCorrectLogin)
                return;

            if (TbName.Text.Length == 0)
                return;

     
            if (!BCorrectPassword)
                return;

            
            if (!BcorrectConfirmPassword)
                return;

            MainWindow.mainWindow.UserLogIn.Login = TbLogin.Text;
            MainWindow.mainWindow.UserLogIn.Password = TbPassword.Password;
            MainWindow.mainWindow.UserLogIn.Name = TbName.Text;
            if (BSetImage){
                MainWindow.mainWindow.UserLogIn.Image = File.ReadAllBytes(FileDialogImage.FileName);
            }
        
            MainWindow.mainWindow.UserLogIn.DateUpdate = DateTime.Now;
         
            MainWindow.mainWindow.UserLogIn.DateCreate = DateTime.Now;
       
            MainWindow.mainWindow.OpenPage(new Confirmation(Confirmation.TypeConfirmation.Regin));
        }
        //пример

        public static void SaveImageToDatabase(string imagePath, int recordId)
        {
            string connectionString = "server=localhost;port=3306;database=regin;user=root;"; // Замените на свою
            byte[] imageData = File.ReadAllBytes(imagePath); // 1. Читаем файл в байты

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string sql = "UPDATE users SET Image = @Image WHERE Id = @Id"; // или INSERT

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    // 2. Добавляем параметр
                    command.Parameters.Add("@Image", MySqlDbType.VarBinary, -1).Value = imageData;
                    command.Parameters.Add("@Id", MySqlDbType.Int32).Value = recordId;

                    // 3. Выполняем запрос
                    command.ExecuteNonQuery();
                }
            }
        }

        //-----------
        private void SetName(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Char.IsLetter(e.Text, 0);
        }
        public void SetNotification(string Message, SolidColorBrush _Color)
        {
            LNameUser.Content = Message;
            LNameUser.Foreground = _Color;
        }
        private void SelectImage(object sender, MouseButtonEventArgs e)
        {
            if (FileDialogImage.ShowDialog() == true)
            {
                using (Imaging.Image image = Imaging.Image.Load(FileDialogImage.FileName))
                {
                    int NewWidht = 0;
                    int NewHeight = 0;
                    if (image.Width > image.Height)
                    {
                        NewWidht = (int)(image.Width * (256f / image.Height));
                        NewHeight = 256;
                    }
                    else
                    {
                        NewWidht = 256;
                        NewHeight = (int)(image.Height * (256f / image.Width));
                    }
                    image.Resize(NewWidht, NewHeight);
                    image.Save("IUsers" + MainWindow.mainWindow.UserLogIn.Id + ".jpg");
                }
            }
            using (Imaging.RasterImage rasterImage = (Imaging.RasterImage)Imaging.Image.Load("IUsers" + MainWindow.mainWindow.UserLogIn.Id + ".jpg"))
            {
                if (!rasterImage.IsCached)
                {
                    rasterImage.CacheData();
                }
                int X = 0;
                int Width = 256;
                int Y = 0;
                int Height = 256;

                if (rasterImage.Width > rasterImage.Height)
                {
                    X = (int)((rasterImage.Width - 256F) / 2);
                }
                else
                {
                    Y = (int)((rasterImage.Height - 256F) / 2);
                }

                Imaging.Rectangle rectangle = new Imaging.Rectangle(X, Y, Width, Height);
                rasterImage.Crop(rectangle);

                rasterImage.Save("IUsers.jpg");

                DoubleAnimation StartAnimation = new DoubleAnimation();
                StartAnimation.From = 1;
                StartAnimation.To = 0;
                StartAnimation.Duration = TimeSpan.FromSeconds(0.6);
                StartAnimation.Completed += delegate
                {
                    IUser.Source = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + @"\IUsers.jpg"));
                    DoubleAnimation EndAnimation = new DoubleAnimation();
                    EndAnimation.From = 0;
                    EndAnimation.To = 1;
                    EndAnimation.Duration = TimeSpan.FromSeconds(1.2);
                    IUser.BeginAnimation(OpacityProperty, EndAnimation);
                };

                IUser.BeginAnimation(OpacityProperty, StartAnimation);
                BSetImage = true;
            }
        }
        private void OpenLogin(object sender, MouseButtonEventArgs e)
        {
            MainWindow.mainWindow.OpenPage(new Login());
        }

        private void OpenRegin(object sender, MouseButtonEventArgs e)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
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
using Microsoft.VisualBasic;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;


namespace RegIN6.Pages
{
    /// <summary>
    /// Логика взаимодействия для PinPage.xaml
    /// </summary> public partial class PinPage : Page
    public partial class PinPage : Page
    {
        // Делаем enum PUBLIC
        public enum PinMode
    {
        Setup,
        Login,
        Change
    }

    private PinMode currentMode;

    // Конструктор теперь принимает PUBLIC enum
    public PinPage(PinMode mode)
    {
        InitializeComponent();
        currentMode = mode;

        SetupPage();
    }

    private void SetupPage()
    {
        switch (currentMode)
        {
            case PinMode.Setup:
                LTitle.Content = "Установите PIN-код";
                BtnAction.Content = "Установить";
                break;

            case PinMode.Login:
                LTitle.Content = "Введите PIN-код";
                BtnAction.Content = "Войти";
                break;

            case PinMode.Change:
                LTitle.Content = "Введите новый PIN-код";
                BtnAction.Content = "Изменить";
                break;
        }

        TbPin.Focus();
    }

    private void TbPin_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        // Разрешаем только цифры
        e.Handled = !char.IsDigit(e.Text, 0);
    }

    private void TbPin_KeyUp(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && TbPin.Password.Length == 4)
        {
            ProcessPin();
        }
    }

    private void BtnAction_Click(object sender, RoutedEventArgs e)
    {
        ProcessPin();
    }

    private void ProcessPin()
    {
        string pin = TbPin.Password;

        // Проверка длины
        if (pin.Length != 4)
        {
            LError.Content = "Введите 4 цифры";
            return;
        }

        // Проверка, что все символы - цифры
        if (!int.TryParse(pin, out _))
        {
            LError.Content = "Только цифры разрешены";
            return;
        }

        // Проверка на простые комбинации
        string[] simplePins = { "0000", "1111", "2222", "3333", "4444",
                                   "5555", "6666", "7777", "8888", "9999",
                                   "1234", "4321" };

        if (Array.Exists(simplePins, p => p == pin))
        {
            LError.Content = "Выберите более сложный PIN";
            return;
        }

        LError.Content = "";

        // Обработка в зависимости от режима
        switch (currentMode)
        {
            case PinMode.Setup:
                SetupPin(pin);
                break;

            case PinMode.Login:
                LoginWithPin(pin);
                break;

            case PinMode.Change:
                ChangePin(pin);
                break;
        }
    }

    private void SetupPin(string pin)
    {
        MainWindow.mainWindow.UserLogIn.SetPin(pin);
        MessageBox.Show("PIN-код успешно установлен!", "Успех",
            MessageBoxButton.OK, MessageBoxImage.Information);

        // Возвращаемся на страницу логина
        MainWindow.mainWindow.OpenPage(new Login());
    }

    private void LoginWithPin(string pin)
    {
        if (MainWindow.mainWindow.UserLogIn.CheckPin(pin))
        {
            MessageBox.Show($"Добро пожаловать, {MainWindow.mainWindow.UserLogIn.Name}!",
                "Успешная авторизация", MessageBoxButton.OK, MessageBoxImage.Information);

            // Здесь можно открыть главное окно приложения
            // MainWindow.mainWindow.OpenPage(new MainPage());
        }
        else
        {
            LError.Content = "Неверный PIN-код";
            TbPin.Password = "";
            TbPin.Focus();
        }
    }

    private void ChangePin(string pin)
    {
        MainWindow.mainWindow.UserLogIn.SetPin(pin);
        MessageBox.Show("PIN-код успешно изменен!", "Успех",
            MessageBoxButton.OK, MessageBoxImage.Information);

        MainWindow.mainWindow.OpenPage(new Login());
    }

    private void OpenLogin(object sender, MouseButtonEventArgs e)
    {
        MainWindow.mainWindow.OpenPage(new Login());
    }
}
}

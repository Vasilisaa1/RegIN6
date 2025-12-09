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

namespace RegIN6.Elements
{
    /// <summary>
    /// Логика взаимодействия для ElementCapture.xaml
    /// </summary>
    public partial class ElementCapture : UserControl
    {
        public CorrectCapture HandlerCorrectCapture;
        public delegate void CorrectCapture();
        string StrCapture = "";
        int ElementWidth = 280;
        int ElementHeight = 50;
        Random ThisRandom = new Random();
        public ElementCapture()
        {
            InitializeComponent();
            CreateCapture();
        }

        public void CreateCapture()
        {
            InputCapture.Text = "";
            Capture.Children.Clear();
            StrCapture = "";

            for (int i = 0; i < 100; i++)
            {
                Label Lbackground = new Label()
                {
                    Content = ThisRandom.Next(0, 10),
                    FontSize = ThisRandom.Next(10, 16),
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush(Color.FromArgb(100, (byte)ThisRandom.Next(0, 255), (byte)ThisRandom.Next(0, 255), (byte)ThisRandom.Next(0, 255))),
                    Margin = new Thickness(ThisRandom.Next(0, ElementWidth - 20), ThisRandom.Next(0, ElementHeight - 20), 0, 0)
                };
                Capture.Children.Add(Lbackground);
            }

            for (int i = 0; i < 4; i++)
            {
                char c = (char)('0' + ThisRandom.Next(0, 10));
                StrCapture += c;

                Label lCode = new Label()
                {
                    Content = c,
                    FontSize = 30,
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush(Color.FromArgb(255, (byte)ThisRandom.Next(0, 255),
                    (byte)ThisRandom.Next(0, 255), (byte)ThisRandom.Next(0, 255))),
                    Margin = new Thickness(ElementWidth / 2 - 60 + i * 30, ThisRandom.Next(-10, 10), 0, 0)
                };

                Capture.Children.Add(lCode);
            }

            InputCapture.Focus();
        }

        public bool OnCapture()
        {
            return StrCapture == InputCapture.Text;
        }

        private void EnterCapture(object sender, KeyEventArgs e)
        {
            if (InputCapture.Text.Length == 4)
            {
                if (!OnCapture())
                {
                    CreateCapture();
                }
                else if (HandlerCorrectCapture != null)
                {
                    HandlerCorrectCapture.Invoke();
                }
            }
        }
    }
}

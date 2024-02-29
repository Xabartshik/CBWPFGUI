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

namespace CBWPFGUI
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string nickname { get; set; }
        public MainWindow()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Процедура ввода никнейма и закрытия окна
        /// </summary>
        private void start()
        {
            string nickname = TBNameInput.Text;
            WindowCB windowCB = new WindowCB(nickname);
            windowCB.Show();
            this.Close();
        }
        //Выполнение start при нажатии на кнопку установки имени
        private void ButtonSetName_Click(object sender, RoutedEventArgs e)
        {
            start();
        }
        //Выполнение start при нажатии Left Ctrl + Enter
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.Enter)
            {
                // Ваш код обработчика для сочетания клавиш Ctrl + Enter
                start();
            }
        }
    }
}

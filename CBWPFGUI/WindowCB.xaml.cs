using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using CBAPI_NS;
//Позволяет использовать методы класса CBApi без указания CBApi
using static CBAPI_NS.CBApi;

namespace CBWPFGUI
{
    /// <summary>
    /// Логика взаимодействия для WindowCB.xaml
    /// </summary>
    public partial class WindowCB : Window
    {
        //Переменные для цвета окна бота и пользователя
        string botMessageColor = "#393a40";
        string userMessageColor= "#521bf7";
        string foreground = "#f5f3f2";
        public WindowCB(string nname)
        {
            nickname = nname;
            InitializeComponent();
        }
        private string nickname;
        /// <summary>
        /// Панель для вставки, Отправитель, Содержимое, Цвет шрифта блока сообщения, Цвет шрифта отправителя, Цвет фона сообщения, ровнение
        /// </summary>
        /// <param name="Panel">Панель для вставки</param>
        /// <param name="sender">Отправитель</param>
        /// <param name="content">Содержимое</param>
        /// <param name="TBMFColor"> Цвет шрифта блока сообщения</param>
        /// <param name="TBSFColor"> Цвет шрифта отправителя</param>
        /// <param name="BBColor">Цвет фона сообщения</param>
        /// <param name="Left">ровнение</param>
        private static void addMessage(StackPanel Panel, string sender, string content, string TBMFColor, string TBSFColor, string BBColor, bool Left)
        {
            if (content == "")
            {
                throw new Exception();
            }
            StackPanel messagePanel = new StackPanel();
            Border messageBorder = new Border
            {
                Background = (SolidColorBrush)new BrushConverter().ConvertFrom(BBColor),
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(10),
                Margin = new Thickness(0, 5, 0, 5)
            };
            // Создать TBlock для имени отправителя
            TextBlock TBSender = new TextBlock
            {
                Text = sender,
                Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom(TBSFColor),
                Margin = new Thickness(0, 0, 5, 0)
            };
            if (Left == false)
            {
                TBSender.HorizontalAlignment = HorizontalAlignment.Right;
            }

            // Создать TextBlock для сообщения
            TextBlock TBMessage = new TextBlock
            {
                Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom(TBMFColor),
                Text = content
            };
            // Добавить TBSender и TBMessage в messagePanel
            messagePanel.Children.Add(TBSender);
            messageBorder.Child = TBMessage;
            messagePanel.Children.Add(messageBorder);
            // Добавить messagePanel в Panel
            Panel.Children.Add(messagePanel);
        }
        /// <summary>
        /// Панель для вставки, Отправитель, Содержимое, Цвет шрифта блока сообщения, Цвет шрифта отправителя, Цвет фона сообщения, Время ОЖИДАНИЯ в секундах
        /// </summary>
        /// <param name="Panel">Панель для вставки</param>
        /// <param name="sender">Отправитель</param>
        /// <param name="content">Содержимое</param>
        /// <param name="TBMFColor"> Цвет шрифта блока сообщения</param>
        /// <param name="TBSFColor"> Цвет шрифта отправителя</param>
        /// <param name="BBColor">Цвет фона сообщения</param>
        /// <param name="Left">ровнение</param>
        /// <param name="wait"> Время ОЖИДАНИЯ в секундах</param>
        private async static void addMessage(StackPanel Panel, string sender, string content, string TBMFColor, string TBSFColor, string BBColor, bool Left, ulong wait)
        {
            if (content == "")
            {
                throw new Exception();
            }
            StackPanel messagePanel = new StackPanel();
            Border messageBorder = new Border
            {
                Background = (SolidColorBrush)new BrushConverter().ConvertFrom(BBColor),
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(10),
                Margin = new Thickness(0, 5, 0, 5)
            };
            // Создать TBlock для имени отправителя
            TextBlock TBSender = new TextBlock
            {
                Text = sender,
                Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom(TBSFColor),
                Margin = new Thickness(0, 0, 5, 0)
            };
            if (Left == false)
            {
                TBSender.HorizontalAlignment = HorizontalAlignment.Right;
            }

            // Создать TextBlock для сообщения
            TextBlock TBMessage = new TextBlock
            {
                Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom(TBMFColor),
                Text = content
            };
            // Добавить TBSender и TBMessage в messagePanel
            messagePanel.Children.Add(TBSender);
            messageBorder.Child = TBMessage;
            messagePanel.Children.Add(messageBorder);
            // Добавить messagePanel в Panel

            await Task.Delay(System.TimeSpan.FromSeconds(wait));
            Panel.Children.Add(messagePanel);
        }


        /// <summary>
        /// Отправка сообщения
        /// </summary>
        private void sendDudes()
        {
            //Проверка на пустоту сообщения
            try
            {
                addMessage(SPDialogue, nickname, TBUser.Text, foreground, foreground, userMessageColor, false);
                addMessage(SPDialogue, "Бот", processMessage(TBUser.Text), foreground, foreground, botMessageColor, true, 1);
                SPDialogue.CanVerticallyScroll = true;
                ScrollChat.ScrollToEnd();
                //TBlMessageStory.Text += TBUser.Text + '\n';
                //TBlMessageStory.Text += processMessage(TBUser.Text) + '\n';

                //TBUser.Text = "";
            }
            catch (Exception ex)
            {

            }
        }
        /// <summary>
        /// Отправка сообщения по нажатию на кнопку
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonSend_Click(object sender, RoutedEventArgs e)
        {
                sendDudes();
        }
        //Отправка сообщения хоткеем
        private void WindowCB_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.Enter)
            {
                // Ваш код обработчика для сочетания клавиш Ctrl + Enter
                sendDudes();
            }
        }
    }
}

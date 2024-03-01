using System;
using System.Collections.Generic;
using System.IO;
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
//Сделал: Ошлаков Данил, ИВТ-22
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
            CBApi.botName = "Shhh Mi$ery";
            nickname = nname;
            InitializeComponent();
            //Если есть json, то он выгружается, а сообщения восстанавливаются
            if (File.Exists(nickname + ".json"))
            {
                LoadJson(nickname);
                foreach (Message message in CBApi.messageStory)
                {
                    addMessage(SPDialogue, message, foreground, foreground, userMessageColor, false);
                    addMessage(SPDialogue, message, foreground, foreground, botMessageColor, true);
                }
                ScrollChat.ScrollToEnd();
            }
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
        private static void addMessage(StackPanel Panel, Message message, string TBMFColor, string TBSFColor, string BBColor, bool Left)
        {
            if (message.Content == "")
            {
                throw new Exception();
            }
            StackPanel messagePanel = new StackPanel();
            StackPanel messageInfoPanel = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(2, 0, 2, 0)
            };
            Border messageBorder = new Border
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Background = (SolidColorBrush)new BrushConverter().ConvertFrom(BBColor),
                CornerRadius = new CornerRadius(0, 10, 10, 10),
                Padding = new Thickness(10),
                Margin = new Thickness(0, 5, 20, 5)
            };
            // Создать TBlock для имени отправителя
            TextBlock TBSender = new TextBlock
            {
                Text = message.Sender,
                Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom(TBSFColor),
                Margin = new Thickness(0, 0, 5, 0)
            };
            // Создать TBlock для имени отправителя
            TextBlock TBTime = new TextBlock
            {
                Text = message.Date + ", " + message.Time,
                Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom(TBSFColor),
                Margin = new Thickness(10, 0, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Right
            };
            if (Left == false)
            {
                TBSender.HorizontalAlignment = HorizontalAlignment.Right;
                TBTime.HorizontalAlignment = HorizontalAlignment.Left;
                messageInfoPanel.HorizontalAlignment = HorizontalAlignment.Right;
                messageBorder.HorizontalAlignment = HorizontalAlignment.Right;
                messageBorder.Margin = new Thickness(20, 5, 0, 5);
                messageBorder.CornerRadius = new CornerRadius(10, 0, 10, 10);
            }

            // Создать TextBlock для сообщения
            TextBlock TBMessage = new TextBlock
            {
                Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom(TBMFColor),
                Text = message.Content,
                TextWrapping = TextWrapping.Wrap
            };
            // Добавить TBSender и TBMessage в messagePanel
            messageInfoPanel.Children.Add(TBSender);
            messageInfoPanel.Children.Add(TBTime);
            messageBorder.Child = TBMessage;
            messagePanel.Children.Add(messageBorder);
            // Добавить messagePanel в Panel
            Panel.Children.Add(messageInfoPanel);
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
        /// <param name="wait"> Время ОЖИДАНИЯ в секундах</param
        private async static void addMessage(StackPanel Panel, Message message, string TBMFColor, string TBSFColor, string BBColor, bool Left, ulong wait)
        {
            if (message.Content == "")
            {
                throw new Exception();
            }
            StackPanel messagePanel = new StackPanel();
            StackPanel messageInfoPanel = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(2, 0, 2, 0)
            };
            Border messageBorder = new Border
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Background = (SolidColorBrush)new BrushConverter().ConvertFrom(BBColor),
                CornerRadius = new CornerRadius(0, 10, 10, 10),
                Padding = new Thickness(10),
                Margin = new Thickness(0, 5, 20, 5)
            };
            // Создать TBlock для имени отправителя
            TextBlock TBSender = new TextBlock
            {
                Text = message.Sender,
                Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom(TBSFColor),
                Margin = new Thickness(0, 0, 5, 0)
            };
            // Создать TBlock для имени отправителя
            TextBlock TBTime = new TextBlock
            {
                Text = message.Date + ", "+ message.Time,
                Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom(TBSFColor),
                Margin = new Thickness(10, 0, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Right
            };
            if (Left == false)
            {
                TBSender.HorizontalAlignment = HorizontalAlignment.Right;
                TBTime.HorizontalAlignment = HorizontalAlignment.Left;
                messageInfoPanel.HorizontalAlignment = HorizontalAlignment.Right;
                messageBorder.HorizontalAlignment = HorizontalAlignment.Right;
                messageBorder.Margin = new Thickness(20, 5, 0, 5);
                messageBorder.CornerRadius = new CornerRadius(10, 0, 10, 10);
            }

            // Создать TextBlock для сообщения
            TextBlock TBMessage = new TextBlock
            {
                Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom(TBMFColor),
                Text = message.Content,
                TextWrapping = TextWrapping.Wrap
            };
            // Добавить TBSender и TBMessage в messagePanel
            messageInfoPanel.Children.Add(TBSender);
            messageInfoPanel.Children.Add(TBTime);
            messageBorder.Child = TBMessage;
            messagePanel.Children.Add(messageBorder);
            // Добавить messagePanel в Panel
            await Task.Delay(System.TimeSpan.FromSeconds(wait));
            Panel.Children.Add(messageInfoPanel);
            Panel.Children.Add(messagePanel);
        }

        /// <summary>
        /// Отправка сообщения
        /// </summary>
        private void sendDudes()
        {
                addMessage(SPDialogue, CBApi.TransferMessage(nickname, TBUser.Text, true), foreground, foreground, userMessageColor, false);
            try
            {

                addMessage(SPDialogue, processMessage(CBApi.TransferMessage(nickname, TBUser.Text, false)), foreground, foreground, botMessageColor, true, 0);
            }
            catch (Exception ex)
            {
                addMessage(SPDialogue, CBApi.TransferMessage(nickname, ex.Message, true), foreground, foreground, botMessageColor, true, 0);
            }
                SPDialogue.CanVerticallyScroll = true;
                ScrollChat.ScrollToEnd();

                TBUser.Text = "";
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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

                SaveMessages(nickname);
                SaveJson(nickname);

        }
    }
}

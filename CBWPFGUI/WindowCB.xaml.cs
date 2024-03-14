using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.RightsManagement;
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
                //Загрузка истории сообщений по никнейму пользователя
                LoadJson(nickname);
                ulong iter = 0;
                foreach (Message message in CBApi.messageStory)
                {
                    //Если сообщение нечетное -- написал бот, иначе пользователь
                    iter++;
                    if (iter % 2 == 1)
                    {
                    addMessage(SPDialogue, message, foreground, foreground, userMessageColor, false);
                    }
                    else
                    {
                    addMessage(SPDialogue, message, foreground, foreground, botMessageColor, true);
                    }
                }
                ScrollChat.ScrollToEnd();
            }
        }
        private string nickname;
        /// <summary>
        /// Панель для вставки, Отправитель, Содержимое, Цвет шрифта блока сообщения, Цвет шрифта отправителя, Цвет фона сообщения, ровнение. 
        /// Она статическая для того, чтобы можно было использовать ее в разных проектах и для разных панелей
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
            //Создание панелей для сообщения и информации о сообщении (дата, время, отправитель)
            StackPanel messagePanel = new StackPanel();
            StackPanel messageInfoPanel = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(2, 0, 2, 0)
            };
            //Создание границ для сообщения (скругление, красивый фон)
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
            //Если флаг слева отсутствует, то выравнивание по правой стороне
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
            //Вывод сообщений в панель, указанную в параметрах
            Panel.Children.Add(messageInfoPanel);
            Panel.Children.Add(messagePanel);
        }
        /// <summary>
        /// Панель для вставки, Отправитель, Содержимое, Цвет шрифта блока сообщения, Цвет шрифта отправителя, Цвет фона сообщения, Время ОЖИДАНИЯ в секундах
        /// Асинхронный - вынос отдельных задач из основного потока
        /// Она статическая для того, чтобы можно было использовать ее в разных проектах и для разных панелей
        /// </summary>
        /// <param name="Panel">Панель для вставки</param>
        /// <param name="sender">Отправитель</param>
        /// <param name="content">Содержимое</param>
        /// <param name="TBMFColor"> Цвет шрифта блока сообщения</param>
        /// <param name="TBSFColor"> Цвет шрифта отправителя</param>
        /// <param name="BBColor">Цвет фона сообщения</param>
        /// <param name="Left">ровнение</param>
        /// <param name="wait"> Время ОЖИДАНИЯ в секундах</param
        /*
         Async и await в C# используются для написания асинхронного кода. Давайте разберемся, что это значит:

Асинхронный код:

Позволяет программе выполнять несколько операций одновременно, не блокируя основной поток выполнения.
Часто используется для задач, связанных с вводом-выводом (I/O), например, сетевыми запросами или чтением/записью файлов.
Такие операции могут занимать время, и асинхронный код позволяет программе продолжать работу, не дожидаясь их завершения.

async:

Ключевое слово async используется для объявления метода асинхронным.
Это сообщает компилятору, что метод может содержать операции, которые могут быть приостановлены и возобновлены позже.
Асинхронный метод должен возвращать Task или Task<T>, где T - тип возвращаемого значения.

await:

Ключевое слово await используется внутри асинхронного метода для приостановки его выполнения, пока ожидаемая асинхронная операция не завершится.
await может использоваться только с объектами типа Task или Task<T>.
Когда асинхронная операция завершается, управление возвращается в асинхронный метод, и выполнение продолжается с инструкции после await.
         
         */
        private async static void addMessage(StackPanel Panel, Message message, string TBMFColor, string TBSFColor, string BBColor, bool Left, ulong wait)
        {
            if (message.Content == "")
            {
                throw new Exception();
            }
            //Создание панелей для сообщения и информации о сообщении (дата, время, отправитель)
            StackPanel messagePanel = new StackPanel();
            StackPanel messageInfoPanel = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(2, 0, 2, 0)
            };
            //Создание границ для сообщения (скругление, красивый фон)
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
            //Если флаг слева отсутствует, то выравнивание по правой стороне
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
            //Приостановка выполнения, пока не выполнится задача ожидания TimeSpan.FromSeconds
            await Task.Delay(System.TimeSpan.FromSeconds(wait));
            //Вывод сообщений в панель, указанную в параметрах
            Panel.Children.Add(messageInfoPanel);
            Panel.Children.Add(messagePanel);
        }

        /// <summary>
        /// Отправка сообщения
        /// </summary>

        private void sendMessage()
        {

            //Добавление сообщения от пользователя
            //Проверка сообщений. Если случится ошибка, то выводится сообщение об ошибке
                try
                {

                    addMessage(SPDialogue, CBApi.TransferMessage(nickname, TBUser.Text, true), foreground, foreground, userMessageColor, false);
                }
                catch (Exception ex)
                {

                    addMessage(SPDialogue, CBApi.TransferMessage(nickname, ex.Message, true), foreground, foreground, userMessageColor, false);
                }
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
            sendMessage();
        }
        //Отправка сообщения хоткеем
        private void WindowCB_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.Enter)
            {
                // Ваш код обработчика для сочетания клавиш Ctrl + Enter
                sendMessage();
            }
        }
        ///Сохранение истории при закрытие окна
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            //Сохранить сообщения в текстовый файл и в JSON
                SaveMessages(nickname);
                SaveJson(nickname);

        }
    }
}

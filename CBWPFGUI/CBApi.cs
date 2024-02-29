using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CBAPI_NS
{
    //TODO:
    /* Написать команды для болтовни (Справка, например)
     * Написать команды для получения числа истории сообщений, сколько сейчас времени, какой сегодня день и т.д.
     * Рандомайзер типа 1D20 и простой rand 100, rand 10 100, а также случайный выбор типа "пиво, водка, молоко"
     * Сохранение истории сообщений в список (возможно, имеет смысл сохранять как текстовый файл и как бинарник отдельно)
     * Загрузка из файла истории после запуска программы
     * Парсинг сайта
     */


    /*В C# ключевое слово internal используется для задания уровня доступа к типам и членам внутри одной сборки. 
     * Если тип или член объявлен с модификатором internal, он становится доступным только внутри той же сборки, в которой он был объявлен. 
     * 
     * 
     */
    /// <summary>
    /// Класс сообщения. Хранит в себе дату отправки, время и содержимое сообщения
    /// </summary>
    internal class Message
    {
        //Содержимое отправленного сообщения
        private string _content;
        public string messageContent 
        { 
            get
            {
                return _content;
            }    
            set
            { 
                _content = value; 
            }
        }
        //Дата. Указывается строкой для того чтобы записывать в лог и выводить при необходимости в сообщении
        public string date { get; set; }
        //Время. Указывается строкой для того чтобы записывать в лог и выводить при необходимости в сообщении
        public string time { get; set; }
        //Отправитель. Указывается строкой для того чтобы записывать в лог и выводить при необходимости в сообщении
        public string sender { get; set; }
        /// <summary>
        /// Возвращает время отправки, дату и содержимое сообщения
        /// </summary>
        /// <returns> </returns>
        public override string ToString()
        {
            return date + time + " [" + sender + "] " +messageContent;
        }


    }
    class CBApi
    {
        ///По моей логике, лучше бы сохранять историю сообщений как поле бота, нежели класса окна. Бот один, история сообщений хранится в нем, все хорошо
        //Список истории собщений
        List<Message> messageStory = new List<Message>();
        /// <summary>
        /// Получение времени в формате СС:ММ:ЧЧ
        /// </summary>
        /// <returns>Время в формате СС:ММ:ЧЧ</returns>
        public static string getTime()
        {
            DateTime currentTime = DateTime.Now;
            return currentTime.ToString("[HH:mm:ss]");
        }
        /// <summary>
        /// Получение даты в формате ДД:ММ:ГГ
        /// </summary>
        /// <returns>Дата в формате ДД:ММ:ГГ </returns>
        public static string GetFormattedDate()
        {
            DateTime currentDate = DateTime.Now;
            return currentDate.ToString("[yyyy|MM|dd]");
        }

        static Dictionary<Regex, ulong> check = new Dictionary<Regex, ulong>()
        {
            { new Regex(@"^[a-zA-Zа-яА-Я]+$"), 0 },
            { new Regex(@"\d+"), 1 },
            { new Regex(@".*@.*"), 2 }
        };



        public delegate string AnswerDelegate();
        static AnswerDelegate[] methods = new AnswerDelegate[]
        {

        };


        //Обработка сообщения
        public static string processMessage(string message) 
        {
            ulong answerID;
            //Если сообщение пустое
            if (message == "")
            {
                throw new ArgumentException();
            }
            //Перебор ключей regexKey в ключах словаря
            foreach (var regexKey in check.Keys)
            {
                //Проверка, подходит ли строка шаблону
                Match match = regexKey.Match(message);
                //Если шаблон подошел, то возвращаем
                if (match.Success)
                {
                    return methods[check[regexKey]]();                    
                }
            }
                return "Я тебя не понимаю";
        }




    }

}

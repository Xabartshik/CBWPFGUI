using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using CBWPFGUI;
using static CBAPI_NS.CBApi;


//Сделал: Ошлаков Данил, ИВТ-22
namespace CBAPI_NS
{



    /*В C# ключевое слово internal используется для задания уровня доступа к типам и членам внутри одной сборки. 
     * Если тип или член объявлен с модификатором internal, он становится доступным только внутри той же сборки, в которой он был объявлен. 
     * 
     * 
     */
    /// <summary>
    /// Класс сообщения. Хранит в себе дату отправки, время и содержимое сообщения
    /// </summary>
    public class Message
    {
        //Содержимое отправленного сообщения
        public string Content { get; set; }
        //Дата. Указывается строкой для того чтобы записывать в лог и выводить при необходимости в сообщении
        public string Date { get; set; }
        //Время. Указывается строкой для того чтобы записывать в лог и выводить при необходимости в сообщении
        public string Time { get; set; }
        //Отправитель. Указывается строкой для того чтобы записывать в лог и выводить при необходимости в сообщении
        public string Sender { get; set; }
        //Получение времени отправки сообщения
        public static string getTime()
        {
            DateTime currentTime = DateTime.Now;
            return currentTime.ToString("HH:mm");
        }
        /// <summary>
        /// Получение даты в формате ДД:ММ:ГГ
        /// </summary>
        /// <returns>Дата в формате ДД:ММ:ГГ </returns>
        public static string GetFormattedDate()
        {
            DateTime currentDate = DateTime.Now;
            return currentDate.ToString("yyyy/MM/dd");
        }
        [JsonConstructor]
        public Message(string content, string date, string time, string sender)
        {
            Content = content;
            Date = date;
            Time = time;
            Sender = sender;
        }
        //Конструктор с автором и контентом
        public Message(string author, string newContent)
        {
            Content = newContent;
            Sender = author;
            Date = GetFormattedDate();
            Time = getTime();
        }
        /// <summary>
        /// Возвращает время отправки, автора, дату и содержимое сообщения
        /// </summary>
        /// <returns> </returns>
        /// 
        public override string ToString()
        {
            return Date + " " + Time + " [" + Sender + "] " + Content;
        }


    }



    // todo: КОММЕНТАРИИ
    // Переделать в матрицу
    class CBApi
    {
        ///По моей логике, лучше бы сохранять историю сообщений как поле бота, нежели класса окна. Бот один, история сообщений хранится в нем, все хорошо
        //Список истории собщений
        static public List<Message> messageStory = new List<Message>();
        //Имя Бота.
        static public string botName { get; set; }

        //Преобразует отправителя и содержимое в объект сообщения
        public static Message TransferMessage(string author, string content, bool addToHistory)
        {
            Message result = new Message(author, content);
            if (addToHistory)
            {
                messageStory.Add(result);
            }
            return result;
        }


        //Удаление из строки опасных символов
        public static string SanitizeFilename(string filename)
        {
            string invalidCharsPattern = @"[\/\\:\*\?<>\|\+\""\']*";
            string sanitizedFilename = Regex.Replace(filename, invalidCharsPattern, "");
            if (string.IsNullOrEmpty(sanitizedFilename.Trim()))
            {
                sanitizedFilename = "Пользователь";
            }
            return sanitizedFilename;
        }

        //Делегат для методов ответа на сообщения
        public delegate Message AnswerDelegate(Message arg);


        //Различные методы для ответа на сообщения. Все ясно из названий
        static private Message hello(Message toAnswer)
        {
            string botAnswer = "Здравствуй, " + toAnswer.Sender + "!";
            Message result = new Message(botName, botAnswer);
            return result;

        }
        //Вывести вопрос как дела?
        static private Message howAreYou(Message toAnswer)
        {
            string botAnswer = $"У меня всё хорошо, спасибо! А как твои дела, {toAnswer.Sender}?";
            Message result = new Message(botName, botAnswer);
            return result;
        }
        //Вывести ответ на благодарность
        static private Message thankYou(Message toAnswer)
        {
            string botAnswer = $"Пожалуйста, {toAnswer.Sender}! Рад был помочь.";
            Message result = new Message(botName, botAnswer);
            return result;
        }
        //Вывести прощание
        static private Message bye(Message toAnswer)
        {
            string botAnswer = $"До свидания, {toAnswer.Sender}! Было приятно общаться.";
            Message result = new Message(botName, botAnswer);
            return result;
        }
        //Вывести день недели
        private static Message GetDayOfWeek(Message toAnswer)
        {
            DateTime today = DateTime.Now;
            string[] daysOfWeek = { "Воскресенье", "Понедельник", "Вторник", "Среда", "Четверг", "Пятница", "Суббота" };
            return new Message(botName, "Сегодня " + daysOfWeek[(int)today.DayOfWeek]);
        }
        //Вывести Дату на текущий момент
        private static Message GetDate(Message toAnswer)
        {
            DateTime today = DateTime.Now;
            string[] daysOfWeek = { "Воскресенье", "Понедельник", "Вторник", "Среда", "Четверг", "Пятница", "Суббота" };
            return new Message(botName, "Сегодня " + Message.GetFormattedDate() + ", " + daysOfWeek[(int)today.DayOfWeek]);
        }
        //Получение времени на текущий момент
        private static Message GetTime(Message toAnswer)
        {
            return new Message(botName, "Сейчас " + Message.getTime());
        }
        //Получить число отправленных сообщений
        private static Message GetLength(Message toAnswer)
        {
            return new Message(botName, "Мы с тобой написали уже " + (messageStory.Count + 1) + " сообщений");
        }
        //Извлечь числа
        public static List<int> extractInt(string input)
        {
            List<int> numbers = new List<int>();
            string[] parts = leaveDigits(input).Split(' ');
            foreach (string part in parts)
            {
                int value;
                if (int.TryParse(part, out value))
                {
                    numbers.Add(value);
                }
            }
            return numbers;
        }
        //Оставить только числа в строке
        public static string leaveDigits(string input)
        {
            string result = string.Empty;
            bool prevSpace = false;
            foreach (char c in input)
            {
                if (char.IsDigit(c))
                {
                    result += c;
                    prevSpace = false;
                }
                else if (!prevSpace)
                {
                    result += ' ';
                    prevSpace = true;
                }
            }
            return result;
        }
        //Рандомайзер для числа в пределах, несколько чисел суммой, просто число
        private static Message Rand(Message toAnswer)
        {
            List<int> numbers = new List<int>();
            string answer = "";
            int index = 0;
            Message result = null;
            Regex[] patterns = new Regex[]
            {
                        new Regex(@"\d*(d|д)\d{1,}"), // Числа с буквой D и 1 или более цифрами
                        new Regex(@"\d{1,}\s*\d*")// Числа, разделенные косой чертой
            };
            string check = toAnswer.Content.ToLower();
            check = check.Replace("ранд ", "");
            // Извлечение цифр из найденных совпадений
            foreach (Regex pattern in patterns)
            {
                //Перебор значений, чтобы понять, какая ситуация
                MatchCollection matches = pattern.Matches(check);
                if (matches.Count > 0)
                {
                    // Извлечение цифр из найденных совпадений
                    foreach (Match match in matches)
                    {
                        numbers = extractInt(match.Value);
                    }
                    break;
                }
                index++;
            }
            // Генератор случайных чисел
            var random = new Random();
            switch (index)
            {
                case 0:
                    // Генерация случайных чисел
                    if (numbers.Count == 1)
                    {
                        // Один элемент в списке
                        answer = GenerateOneRandomNumber(numbers[0]);
                    }
                    //Генерация суммы случайных чисел
                    else if (numbers.Count == 2)
                    {
                        // Два элемента в списке
                        answer = GenerateSumOfRandomNumbers(numbers[0], numbers[1]);
                    }
                    break;
                case 1:
                    // Выбор числа из диапазона
                    if (numbers.Count == 1)
                    {
                        // Один элемент в списке
                        answer = GenerateOneRandomNumber(numbers[0]);
                    }
                    else if (numbers.Count == 2)
                    {
                        // Два элемента в списке
                        answer = GenerateRandomNumberInRange(numbers[0], numbers[1]);
                    }
                    else
                    {
                        throw new Exception("Неверный формат команды!");
                    }
                    break;
                default:
                    throw new Exception("Неверный формат команды!");
            }
            if (index == (patterns.Length))
            {
                result = new Message(botName, "Что-то пошло не так... Проверь написание команды");
            }
            else
            {
                result = new Message(botName, "Случайное число: " + answer);
            }
            return result;

        }
        //Случайное число до maxValue квлючительтно
        public static string GenerateOneRandomNumber(int maxValue)
        {
            Random random = new Random();
            return random.Next(1, maxValue + 1).ToString();
        }
        //Сумма count случайных чисел до maxValue
        public static string GenerateSumOfRandomNumbers(int count, int maxValue)
        {
            Random random = new Random();
            int sum = 0;
            string answer = "";
            for (int i = 0; i < count; i++)
            {
                int val = random.Next(1, maxValue + 1);
                sum += val;
                answer += (i != count - 1) ? $"{val} + " : $"{val}";
            }
            return answer += $" = {sum}";
        }
        //Случайное число в диапазоне от 1 до maxValue квлючительтно
        public static string GenerateRandomNumberInRange(int minValue, int maxValue)
        {
            Random random = new Random();
            return random.Next(minValue, maxValue + 1).ToString();
        }
        //Вывести погоду в Чите
        private static Message getWeather(Message toAnswer)
        {
            return new Message(botName, WeatherParse.GetWeather("Chita"));
        }

        //Вывести справку
        private static Message help(Message toAnswer)
        {
            string help = "Привет: Приветствие пользователя" + '\n' + "Пока: Прощание с пользователем"  +'\n' +
"Как дела? Спрашивает пользователя о его делах" + '\n' +
"Спасибо: Выражает благодарность пользователю" + '\n' +
"День недели: Возвращает текущий день недели" + '\n' +
"Дата: Возвращает текущую дату" + '\n' +
"Время: Возвращает текущее время" + '\n' +
"Количество сообщений: Возвращает количество сообщений в чате" + '\n' +
"Ранд: Генерирует случайное число в указанном диапазоне (например, "+"Ранд 1d20"+" сгенерирует случайное число от 1 до 20)" + '\n' +
"Погода: Возвращает текущую погоду в Чите" + '\n' +
"Умножь (*): Умножает два числа (например, Умножь 2 3 вернет 6)" + '\n' +
"Сложи (+): Складывает два числа (например, Сложи 2 3 вернет 5)" + '\n' +
"Вычти (-): Вычитает одно число из другого (например, Вычти 5 2 вернет 3)" + '\n' +
"Раздели (/): Делит одно число на другое (например, Раздели 6 2 вернет 3)"
                ;
            return new Message(botName, help);
        }
        //Вывести сумму двух чисел
        private static Message sum(Message toAnswer)
        {
            List<int> list = new List<int>();
            list = extractInt(toAnswer.Content);
            string result = "Что-то пошло не так... Проверьте операнды";
            if (list.Count == 2)
            {
                result = "Сумма = " + (list[0] + list[1]).ToString();
            }
            return new Message(botName, result);
        }
        //Вывести произведение двух чисел
        private static Message mul(Message toAnswer)
        {

            List<int> list = new List<int>();
            list = extractInt(toAnswer.Content);
            string result = "Что-то пошло не так... Проверьте операнды";
            if (list.Count == 2)
            {
                result = "Произведение = " + (list[0] * list[1]).ToString();
            }
            return new Message(botName, result);
        }
        //Вывести частное и остаток двух чисел
        private static Message div(Message toAnswer)
        {
            List<int> list = new List<int>();
            list = extractInt(toAnswer.Content);
            string result = "Что-то пошло не так... Проверьте операнды";
            if (list.Count == 2)
            {
                result = "Целая часть = "+ (list[0] / list[1]).ToString() + "; Остаток = " + (list[0] % list[1]).ToString();
            }
            return new Message(botName, result);
        }
        //Вывести разницу двух чисел
        private static Message sub(Message toAnswer)
        {
            List<int> list = new List<int>();
            list = extractInt(toAnswer.Content);
            string result = "Что-то пошло не так... Проверьте операнды";
            if (list.Count == 2)
            {
                result = "Разность = " + (list[0] - list[1]).ToString();
            }
            return new Message(botName, result);
        }

        ////Переписать под матрицу
        ////Словарь из регулярок и делегатов
        //static Dictionary<Regex, AnswerDelegate> checkMethod = new Dictionary<Regex, AnswerDelegate>()
        //{
        //    { new Regex(@"^.*(привет)|(здравствуй.{0,2}).*$"), hello},
        //    { new Regex(@"^.*(пока)|(свид.*).*$"), bye},
        //    { new Regex(@"^.*(как.* дела)|(как жи.*).*$"), howAreYou},
        //    { new Regex(@"^.*(благод)|(спасиб).*$"), thankYou},
        //    { new Regex(@"^.*(день недел).*$"), GetDayOfWeek},
        //    { new Regex(@"^.*(сегодн.* числ)|(сегодн.* ден).*$"), GetDate},
        //    { new Regex(@"^.*(сейчас.* врем)|(сколько.* врем).*$"), GetTime},
        //    { new Regex(@"^.*((сколько.*)|(число.*) сообщ)|(сколько.* (общаем.)|(говор))|(истор.* сообщ).*$"), GetLength},
        //    { new Regex(@"^.*(rand.*)|(roll.*)|(dice.*)|(ранд.*)|(ролл.*).*$"), Rand},
        //    { new Regex(@"^.*(погод.*)|(окн.*).*$"), getWeather},
        //    { new Regex(@"^.*(множ.*)|(\d{1,}\s*\*\s*\d{1,}).*$"), mul},
        //    { new Regex(@"^.*(слож.*)|(сум.*)|(\d{1,}\s*\+\s*\d{1,}).*$"), sum},
        //    { new Regex(@"^.*(вычти.*)|(вычит.*)|(\d{1,}\s*\-\s*\d{1,}).*$"), sub},
        //    { new Regex(@"^.*(деле.*)|(дели.*)|(\d{1,}\s*\/\s*\d{1,}).*$"), div},
        //    { new Regex(@"^.*(справ.*)|(помо.*).*$"), help},
        //};


        ////Обработка сообщения
        //public static Message processMessage(Message message)
        //{
        //    Message result = null;
        //    //Если сообщение пустое
        //    if (message.Content == "")
        //    {
        //        throw new ArgumentException();
        //    }
        //    //Перебор ключей regexKey в ключах словаря
        //    foreach (var regexKey in checkMethod.Keys)
        //    {
        //        //Проверка, подходит ли строка шаблону
        //        Match match = regexKey.Match(message.Content.ToLower());
        //        //Если шаблон подошел, то возвращаем
        //        if (match.Success)
        //        {
        //            result = checkMethod[regexKey](message);
        //            messageStory.Add(result);
        //            return result;
        //        }
        //    }
        //    result = new Message(botName, "Извини, но я тебя не понимаю");
        //    messageStory.Add(result);
        //    return result;
        //}
        static Regex[] regexes = new Regex[]
        {
            new Regex(@"^.*(привет)|(здравствуй.{0,2}).*$"),
            new Regex(@"^.*(пока)|(свид.*).*$"),
            new Regex(@"^.*(как.* дела)|(как жи.*).*$"),
            new Regex(@"^.*(благод)|(спасиб).*$"),
            new Regex(@"^.*(день недел).*$"),
            new Regex(@"^.*(сегодн.* числ)|(сегодн.* ден).*$"),
            new Regex(@"^.*(сейчас.* врем)|(сколько.* врем).*$"),
            new Regex(@"^.*((сколько.*)|(число.*) сообщ)|(сколько.* (общаем.)|(говор))|(истор.* сообщ).*$"),
            new Regex(@"^.*(rand.*)|(roll.*)|(dice.*)|(ранд.*)|(ролл.*).*$"),
            new Regex(@"^.*(погод.*)|(окн.*).*$"),
            new Regex(@"^.*(множ.*)|(\d{1,}\s*\*\s*\d{1,}).*$"),
            new Regex(@"^.*(слож.*)|(сум.*)|(\d{1,}\s*\+\s*\d{1,}).*$"),
            new Regex(@"^.*(вычти.*)|(вычит.*)|(\d{1,}\s*\-\s*\d{1,}).*$"),
            new Regex(@"^.*(деле.*)|(дели.*)|(\d{1,}\s*\/\s*\d{1,}).*$"),
            new Regex(@"^.*(справ.*)|(помо.*).*$"),
        };

        static AnswerDelegate[] answerDelegates = new AnswerDelegate[]
        {
            hello,
            bye,
            howAreYou,
            thankYou,
            GetDayOfWeek,
            GetDate,
            GetTime,
            GetLength,
            Rand,
            getWeather,
            mul,
            sum,
            sub,
            div,
            help,
        };
        public static Message processMessage(Message message)
        {
            // Инициализация ответа
            Message result = null;

            // Проверка на пустое сообщение
            if (string.IsNullOrEmpty(message.Content))
            {
                throw new ArgumentException("Пустое сообщение");
            }

            // Преобразование сообщения в нижний регистр
            string lowerContent = message.Content.ToLower();

            // Перебор regexes
            for (int i = 0; i < regexes.Length; i++)
            {
                // Проверка, подходит ли regex
                if (regexes[i].Match(lowerContent).Success)
                {
                    // Вызов делегата и возврат результата
                    Message result_messsage = answerDelegates[i].Invoke(message);
                    result = result_messsage;
                    messageStory.Add(result);
                    return result;
                }
            }

            // Если ни один regex не подошел, то формируем ответ о непонимании
            result = new Message(botName, "Извини, но я тебя не понимаю");
            messageStory.Add(result);

            return result;
        }


        /// Сохранение сообщений в файл текстовый
        public static void SaveMessages(string nickname)
        {
            try
            {
                //Использование StreamWriter для записи сообщений в текстовый файл
                using (StreamWriter writer = new StreamWriter(nickname + ".txt"))
                {
                    foreach (Message message in messageStory)
                    {
                        writer.WriteLine(message.ToString());
                    }
                }
            }
            //Если записать не удалось
            catch (Exception ex)
            {
                throw new Exception("Не удалось сохранить сообщения", ex);
            }
        }

        //Сохранение сообщений в файл JSON
        public static void SaveJson(string nickname)
        {
            try
            {
                //Использование сериализатора
                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                // Сериализуем список объектов в строку JSON
                string json = JsonSerializer.Serialize(messageStory, options);

                // Записываем JSON-строку в файл
                File.WriteAllText(nickname + ".json", json);
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось сохранить сообщения", ex);
            }
        }
        //Загрузка из JSON
        public static void LoadJson(string nickname)
        {
            // Читаем JSON-строку из файла
            string json = File.ReadAllText(nickname + ".json");

            // Десериализуем JSON-строку в список объектов
            messageStory = JsonSerializer.Deserialize<List<Message>>(json);
        }


    }

}

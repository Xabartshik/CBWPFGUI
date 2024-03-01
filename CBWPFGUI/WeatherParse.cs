using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

//Сделал: Ошлаков Данил, ИВТ-22

namespace CBWPFGUI
{
    public static class WeatherParse
    {
        public static string GetWeather(string city)
        {
            // API ключ
            string api_key = "fb20be23c8ebdec3a848edf912ebf11a";
            // URL API с параметрами
            string api_url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={api_key}&units=metric";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Отправляем GET-запрос по URL API
                    HttpResponseMessage response = client.GetAsync(api_url).Result;

                    // Если код ответа успешный (200-299)
                    if (response.IsSuccessStatusCode)
                    {
                        // Читаем ответ в виде строки
                        string content = response.Content.ReadAsStringAsync().Result;

                        // Десериализуем JSON ответ в объект WeatherData
                        var weatherData = JsonSerializer.Deserialize<WeatherData>(content);

                        // Извлекаем температуру из объекта
                        double temperature = weatherData?.main?.temp ?? 0;

                        // Возвращаем строку с информацией о погоде
                        return $"Температура воздуха в {city} сейчас: {temperature}°C";
                    }

                    else
                    {
                        // Возвращаем сообщение о неудаче
                        return "Не удалось получить данные о погоде.";
                    }
                }
            }
            catch (HttpRequestException e)
            {
                // Возвращаем сообщение об ошибке
                return $"Произошла ошибка при получении данных: {e.Message}";
            }
        }
    }

    public class WeatherData
    {
        public Main main { get; set; }
    }

    public class Main
    {
        public double temp { get; set; }
    }
}
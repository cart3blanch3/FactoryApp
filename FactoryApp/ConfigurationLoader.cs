using Newtonsoft.Json;
using System;
using System.IO;

namespace FactoryApp
{
    // Класс ConfigurationLoader предоставляет методы для загрузки и сохранения конфигурации предприятия.
    public class ConfigurationLoader
    {
        // Метод для загрузки данных из файла конфигурации.
        public static Configuration? LoadConfiguration(string filePath)
        {
            try
            {
                string json = File.ReadAllText(filePath); // Чтение содержимого файла в виде JSON строки
                Configuration? configuration = JsonConvert.DeserializeObject<Configuration>(json); // Десериализация JSON в объект Configuration
                return configuration; // Возвращение загруженной конфигурации
            }
            catch (Exception ex)
            {
                // Заменяем обработку стандартного исключения на выброс пользовательского исключения FactoryException
                throw new FactoryException($"Ошибка при загрузке конфигурации: {ex.Message}", ex);
            }
        }

        // Метод для сохранения конфигурации в файл.
        public static void SaveConfiguration(Configuration configuration, string filePath)
        {
            try
            {
                // Сериализуем объект Configuration в JSON.
                string json = JsonConvert.SerializeObject(configuration, Formatting.Indented);

                // Записываем JSON в файл.
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                // Заменяем обработку стандартного исключения на выброс пользовательского исключения FactoryException
                throw new FactoryException($"Ошибка при сохранении конфигурации: {ex.Message}", ex);
            }
        }
    }
}

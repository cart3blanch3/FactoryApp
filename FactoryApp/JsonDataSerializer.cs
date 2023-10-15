// Класс JsonDataSerializer<T> предоставляет функциональность сериализации и десериализации объектов типа T в формат JSON.

using Newtonsoft.Json;

public class JsonDataSerializer<T> : IDataSerializer<T>
{
    // Метод Serialize выполняет сериализацию объекта типа T в формат JSON и сохраняет его в указанный файл.

    public void Serialize(string filePath, T data)
    {
        // Проверяем, что входные данные не равны null, чтобы избежать ошибок.

        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        // Сериализуем объект в формат JSON с отступами для читаемости.

        string json = JsonConvert.SerializeObject(data, Formatting.Indented);

        // Записываем JSON в указанный файл.

        File.WriteAllText(filePath, json);
    }

    // Метод Deserialize выполняет чтение JSON-данных из указанного файла и десериализацию их в объект типа T.

    public T Deserialize(string filePath)
    {
        // Проверяем, существует ли указанный файл.

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("Файл не найден.", filePath);
        }

        // Читаем содержимое файла.

        string json = File.ReadAllText(filePath);

        // Десериализуем JSON-данные в объект типа T и возвращаем его.

        return JsonConvert.DeserializeObject<T>(json);
    }
}

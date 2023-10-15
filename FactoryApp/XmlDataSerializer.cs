using System.Xml.Serialization;

public class XmlDataSerializer<T> : IDataSerializer<T>
{
    // Метод Serialize выполняет сериализацию объекта типа T в формат XML и сохраняет его в указанный файл.

    public void Serialize(string filePath, T data)
    {
        // Проверяем, что входные данные не равны null, чтобы избежать ошибок.

        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        // Создаем XML-сериализатор для типа T.

        XmlSerializer serializer = new XmlSerializer(typeof(T));

        // Используем using для автоматического закрытия файла после записи.

        using (TextWriter writer = new StreamWriter(filePath))
        {
            serializer.Serialize(writer, data);
        }
    }

    // Метод Deserialize выполняет чтение XML-данных из указанного файла и десериализацию их в объект типа T.

    public T Deserialize(string filePath)
    {
        // Проверяем, существует ли указанный файл.

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("Файл не найден.", filePath);
        }

        // Создаем XML-сериализатор для типа T.

        XmlSerializer serializer = new XmlSerializer(typeof(T));

        // Используем using для автоматического закрытия файла после чтения.

        using (TextReader reader = new StreamReader(filePath))
        {
            return (T)serializer.Deserialize(reader);
        }
    }
}
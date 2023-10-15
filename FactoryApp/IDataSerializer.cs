// Интерфейс IDataSerializer<T> определяет контракт для сериализации и десериализации объектов типа T.
public interface IDataSerializer<T>
{
    // Метод Serialize выполняет сериализацию объекта типа T в файл по указанному пути.
    void Serialize(string filePath, T data);

    // Метод Deserialize выполняет десериализацию объекта типа T из файла по указанному пути.
    T Deserialize(string filePath);
}

public class ConsoleFilePrinter
{
    private readonly string logFilePath; // Путь к файлу для записи сообщений
    private readonly object fileLock = new object(); // Объект-замок для синхронизации доступа к файлу

    // Конструктор класса, принимающий путь к файлу
    public ConsoleFilePrinter(string logFilePath)
    {
        this.logFilePath = logFilePath;
    }

    // Метод для печати сообщения в консоль и записи его в файл
    public void PrintToConsoleAndFile(string message)
    {
        // Печатаем сообщение в консоль
        Console.WriteLine(message);

        // Блокируем доступ к файлу
        lock (fileLock)
        {
            // Печатаем сообщение в файл, используя StreamWriter
            using (StreamWriter writer = new StreamWriter(logFilePath, true))
            {
                writer.WriteLine(message); // Записываем сообщение в файл
            }
        }
    }
}

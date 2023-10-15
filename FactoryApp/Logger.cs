using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryApp
{
    // Класс для логгирования основных процессов предприятия
    public class Logger
    {
        // Создаем единственный экземпляр класса Logger
        private static readonly Logger instance = new Logger();

        // Событие для логгирования
        public event Action<string>? LogEvent;

        // Закрытый конструктор, чтобы предотвратить создание других экземпляров
        private Logger()
        {
        }

        // Статический метод для доступа к единственному экземпляру логгера
        public static Logger Instance => instance;

        // Статический метод для логгирования сообщения
        public static void Log(string message)
        {
            // Вызываем событие для логгирования
            instance.LogEvent?.Invoke(message);
        }
    }
}

namespace FactoryApp
{
    // Класс FactoryException, представляющий пользовательское исключение в приложении.
    public class FactoryException : Exception
    {
        // Пустой конструктор класса FactoryException.
        public FactoryException() { }

        // Конструктор класса FactoryException, принимающий сообщение об ошибке.
        public FactoryException(string message) : base(message)
        {
        }

        // Конструктор класса FactoryException, принимающий сообщение об ошибке и вложенное системное исключение.
        public FactoryException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

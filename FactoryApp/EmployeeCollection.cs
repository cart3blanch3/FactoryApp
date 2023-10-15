using System.Collections;

namespace FactoryApp
{
    // Класс EmployeeCollection<T> представляет собой обобщенную коллекцию, которая содержит сотрудников предприятия.
    // Тип T должен быть производным от класса Employee.
    public class EmployeeCollection<T> : ICollection<T> where T : Employee
    {
        private List<T> items = new List<T>(); // Лист элементов коллекции.

        public int Count => items.Count; // Количество элементов в коллекции.
        public bool IsReadOnly => false; // Свойство, указывающее, доступна ли коллекция только для чтения.

        public event Action<string> SortingStarted; // Событие, сигнализирующее о начале сортировки коллекции.
        public event Action<int> SortingCompleted; // Событие, сигнализирующее об окончании сортировки коллекции с указанием количества обработанных элементов.

        // Конструктор по умолчанию.
        public EmployeeCollection() { }

        // Конструктор, который принимает коллекцию элементов и добавляет их в эту коллекцию.
        public EmployeeCollection(IEnumerable<T> collection)
        {
            items.AddRange(collection);
        }

        // Добавление элемента в коллекцию.
        public void Add(T item)
        {
            items.Add(item);
        }

        // Очистка коллекции.
        public void Clear()
        {
            items.Clear();
        }

        // Проверка наличия элемента в коллекции.
        public bool Contains(T item)
        {
            return items.Contains(item);
        }

        // Копирование элементов коллекции в массив.
        public void CopyTo(T[] array, int arrayIndex)
        {
            items.CopyTo(array, arrayIndex);
        }

        // Удаление элемента из коллекции.
        public bool Remove(T item)
        {
            return items.Remove(item);
        }

        // Реализация интерфейса IEnumerable<T>.
        public IEnumerator<T> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        // Реализация интерфейса IEnumerable.
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        // Асинхронная сортировка коллекции с использованием делегата Action.
        public async Task SortAsync(Action<List<T>> sortAction)
        {
            // Вызываем событие о начале сортировки.
            SortingStarted?.Invoke("Начало сортировки");

            await Task.Run(() => sortAction(items)); // Запуск сортировки в отдельном потоке.

            // Вызываем событие о завершении сортировки с количеством обработанных элементов.
            SortingCompleted?.Invoke(items.Count);
        }

        // Сравнение элементов с использованием делегата Func.
        public bool Compare(Func<T, T, bool> compareFunc)
        {
            return items.All(x => compareFunc(x, items[0]));
        }

        // Поиск свободного работника заданного типа.
        public T FindAvailableEmployee(EmployeeType type)
        {
            return items.FirstOrDefault(x => !x.IsBusy && x.Type == type);
        }
    }
}

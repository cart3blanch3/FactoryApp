using FactoryApp;

// Класс Carpenter (Столяр), наследующийся от Employee
public class Carpenter : Employee
{
    // Событие для оповещения о завершении производства
    public event Action<Order>? ProductionCompleted;

    // Свойство для отслеживания количества произведенных изделий
    public int ProducedItemCount { get; set; } = 0;

    // Переопределенное свойство Salary, вычисляющее зарплату
    public new decimal Salary => CalculateSalary();

    // Конструктор класса Carpenter, инициализирующий экземпляр столяра с указанным именем и предприятием.
    public Carpenter(string name) : base(name, EmployeeType.Carpenter)
    {
    }

    // Пустой конструктор для XML сериализации
    public Carpenter() : base(string.Empty, EmployeeType.Carpenter)
    {
    }

    // Переопределение метода для вычисления зарплаты на основе количества произведенных изделий
    public override decimal CalculateSalary()
    {
        return 1000 + ProducedItemCount * 125.5m;
    }

    // Создание объекта блокировки для синхронизации доступа к ресурсам
    private readonly object machineLock = new object();

    // Асинхронный метод для производства мебели на основе заказа.
    public async Task ProduceFurnitureAsync(Order order, Enterprise enterprise)
    {
        // Проверка, что заказ не является null
        if (order == null)
        {
            throw new FactoryException("Заказ не может быть null.");
        }

        var quantity = order.Quantity; // Количество мебели в заказе
        var furnitureType = order.FurnitureType; // Тип мебели в заказе
        var materialType = order.MaterialType; // Тип материала для мебели в заказе

        // Логгирование получения заказа на производство
        Logger.Log($"Получен заказ на производство {quantity} {furnitureType} из {materialType}.");
        IsBusy = true; // Установка статуса "занят"

        if (enterprise == null)
        {
            throw new FactoryException("Не удалось получить доступ к предприятию.");
        }

        var warehouse = enterprise.GetWarehouse(); // Получение доступа к складу предприятия

        if (warehouse == null)
        {
            throw new FactoryException("Не удалось получить доступ к складу.");
        }

        var furnitureMaterial = warehouse.GetMaterialByType(materialType); // Получение материала со склада
        var furnitureInfo = Furniture.GetFurnitureInfo(furnitureType); // Получение информации о мебели
        int materialQuantity = furnitureInfo.Item1; // Количество материала для каждой единицы мебели
        int productionTime = furnitureInfo.Item2; // Время производства одной единицы мебели

        // Выполняем цикл до тех пор, пока не выполним заказ
        while (quantity > 0)
        {
            // Проверяем существование материала на складе
            if (furnitureMaterial != null)
            {
                // Проверяем достаточно ли материала на складе
                if (!warehouse.HasEnoughRawMaterial(furnitureMaterial, materialQuantity))
                {
                    Logger.Log($"Недостаточно сырья на складе. Ожидание пополнения ресурсов."); // Логгирование ожидания сырья
                    await Task.Delay(5000); // Подождать 5 секунд и повторить попытку
                    continue; // Перейти к следующей итерации цикла
                }

                var machines = enterprise.GetMachines(); // Получение доступа к станкам предприятия
                Machine? freeMachine = null;

                // Используем блокировку, чтобы синхронизировать доступ при поиске свободного станка
                lock (machineLock)
                {
                    freeMachine = machines.FirstOrDefault(machine => !machine.IsOccupied && !machine.IsBroken); // Поиск свободного рабочего станка
                }

                if (freeMachine == null)
                {
                    Logger.Log("Нет доступных станков. Ожидание свободного станка."); // Логгирование ожидания станка
                    await Task.Delay(5000); // Подождать 5 секунд и повторить попытку
                    continue; // Перейти к следующей итерации цикла
                }
                else
                {
                    // Используем блокировку, чтобы синхронизировать доступ при использовании станка
                    lock (machineLock)
                    {
                        freeMachine.Use(); // Использование станка
                    }

                    await Task.Delay(productionTime); // Имитация времени производства
                    warehouse.RemoveRawMaterial(furnitureMaterial, materialQuantity); // Удаление сырья со склада
                    var newFurniture = new Furniture(furnitureType, furnitureMaterial); // Создание новой мебели
                    Logger.Log($"Столяр: {Id} произвел {newFurniture}."); // Логгирование производства мебели
                    ProducedItemCount++; // Увеличиваем количество произведенных столяром изделий 

                    // Используем блокировку, чтобы синхронизировать доступ при освобождении станка
                    lock (machineLock)
                    {
                        freeMachine.Release(); // Освобождение станка
                    }

                    warehouse.AddFinishedProduct(newFurniture, 1); // Добавление готовой мебели на склад
                    quantity--; // Уменьшение оставшегося количества мебели в заказе
                }
            }
            else
            {
                throw new FactoryException("Неизвестный тип сырья");
            }
        }

        // Вызов события завершения производства с передачей заказа
        ProductionCompleted?.Invoke(order);
        Logger.Log($"Столяр: {Id} выполнил заказ {order.Quantity} {order.FurnitureType} из {order.MaterialType}.");
        IsBusy = false; // Установка статуса "свободен" после завершения производства
    }
}

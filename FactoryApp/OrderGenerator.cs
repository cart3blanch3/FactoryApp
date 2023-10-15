using System;
using System.Threading.Tasks;

namespace FactoryApp
{
    // Класс для генерации случайных заказов
    public class OrderGenerator
    {
        private static Random random = new Random(); // Статический объект Random для генерации случайных значений

        // Метод для генерации случайного заказа
        private static Order GenerateRandomOrder()
        {
            // Генерация случайного типа мебели из перечисления FurnitureType
            FurnitureType furnitureType = (FurnitureType)random.Next(Enum.GetValues(typeof(FurnitureType)).Length);

            // Генерация случайного типа материала из перечисления MaterialType
            MaterialType materialType = (MaterialType)random.Next(Enum.GetValues(typeof(MaterialType)).Length);

            // Генерация случайного количества мебели от 1 до 10
            int quantity = random.Next(1, 10);

            // Создание объекта заказа на основе сгенерированных значений
            Order order = new Order(furnitureType, materialType, quantity);
            return order;
        }

        // Метод для асинхронной генерации случайных заказов и добавления их в предприятие
        public async Task GenerateRandomOrdersAsync(int count, Enterprise enterprise)
        {
            try
            {
                for (int i = 0; i < count; i++)
                {
                    Order order = GenerateRandomOrder(); // Генерация случайного заказа
                    await Task.Delay(random.Next(1000, 2000)); // Асинхронная задержка для имитации процесса генерации
                    enterprise.EnqueueOrder(order); // Добавление заказа в очередь предприятия
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"{ex.Message}");
            }
        }
    }
}

namespace FactoryApp
{
    // Класс заказа
    public class Order
    {
        public int OrderId { get; private set; } // Уникальный идентификатор заказа
        public FurnitureType FurnitureType { get; private set; } // Тип мебели в заказе
        public MaterialType MaterialType { get; private set; } // Тип материала для мебели в заказе
        public int Quantity { get; private set; } // Количество мебели в заказе
        public decimal TotalPrice { get; set; } // Общая стоимость заказа

        private static int nextOrderId = 1; // Статическая переменная для генерации уникальных идентификаторов заказов

        // Конструктор класса Order
        public Order(FurnitureType furnitureType, MaterialType materialType, int quantity)
        {
            OrderId = nextOrderId++; // Генерация уникального идентификатора заказа
            FurnitureType = furnitureType; // Установка типа мебели в заказе
            MaterialType = materialType; // Установка типа материала для мебели в заказе
            Quantity = quantity; // Установка количества мебели в заказе
            TotalPrice = Furniture.CalculatePrice(furnitureType, materialType); // Вычисление общей стоимости заказа на основе типа мебели и материала
        }
    }
}

namespace FactoryApp
{
    // Перечисление для типов мебели
    public enum FurnitureType
    {
        Table,    // Стол
        Chair,    // Стул
        Wardrobe  // Шкаф
    }

    // Класс мебели
    public class Furniture
    {
        public Material Material { get; private set; } // Материал, из которого сделана мебель
        public FurnitureType Type { get; private set; } // Тип мебели (стол, стул, шкаф)
        public decimal Price { get; private set; } // Цена мебели
        public int ProductionTimeInMilliseconds { get; private set; } // Время производства в миллисекундах
        public int MaterialQuantity { get; private set; } // Количество материала, необходимое для производства

        // Конструктор класса Furniture, создающий экземпляр мебели с указанным типом и материалом
        public Furniture(FurnitureType type, Material material)
        {
            Type = type; // Присвоение типа мебели
            Material = material; // Присвоение материала
            Price = CalculatePrice(type, material.Type); // Расчет цены на основе типа мебели и материала
            (MaterialQuantity, ProductionTimeInMilliseconds) = GetFurnitureInfo(type); // Получение информации о необходимом материале и времени производства
        }

        // Метод для расчета цены товара на основе типа товара и типа материала
        public static decimal CalculatePrice(FurnitureType furnitureType, MaterialType materialType)
        {
            (int materialQuantity, _) = GetFurnitureInfo(furnitureType); // Получаем количество материала для данного типа мебели
            decimal unitPrice = Material.CalculatePricePerUnit(materialType); // Получаем цену за единицу материала
            decimal totalPrice = unitPrice * materialQuantity;
            return totalPrice + 100; // Добавляем фиксированную наценку в 100 единиц валюты
        }

        // Метод для получения информации о материале и времени производства для указанного типа мебели
        public static (int materialQuantity, int productionTime) GetFurnitureInfo(FurnitureType furnitureType)
        {
            return furnitureType switch
            {
                FurnitureType.Table => (4, 1000), // Для стола требуется 4 единицы материала и время производства 1000 миллисекунд
                FurnitureType.Chair => (2, 500),  // Для стула требуется 2 единицы материала и время производства 500 миллисекунд
                FurnitureType.Wardrobe => (6, 1500), // Для шкафа требуется 6 единиц материала и время производства 1500 миллисекунд
                _ => throw new FactoryException("Не удалось найти информацию для указанного типа мебели.")
            };
        }

        // Переопределение метода Equals для сравнения двух экземпляров мебели
        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Furniture other = (Furniture)obj;
            return Type == other.Type && Material == other.Material;
        }

        // Переопределение метода GetHashCode для получения хэш-кода мебели
        public override int GetHashCode()
        {
            return (Type, Material).GetHashCode();
        }

        // Переопределение метода ToString для получения текстового представления мебели
        public override string ToString()
        {
            return $"{Type} из {Material.Type}";
        }
    }
}

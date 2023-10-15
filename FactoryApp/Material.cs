namespace FactoryApp
{
    // Перечисление для типов материалов
    public enum MaterialType
    {
        Oak,    // Дуб
        Pine,   // Сосна
        Birch,  // Береза
        Maple   // Клен
    }

    // Класс сырья
    public class Material
    {
        // Тип материала
        public MaterialType Type { get; private set; }

        // Цена за единицу сырья
        public decimal PricePerUnit { get; private set; }

        // Конструктор для создания экземпляра класса Material
        public Material(MaterialType type)
        {
            Type = type;
            // Определение цены за единицу сырья на основе его типа
            PricePerUnit = CalculatePricePerUnit(type);
        }

        // Метод для вычисления цены за единицу сырья на основе его типа
        public static decimal CalculatePricePerUnit(MaterialType type)
        {
            switch (type)
            {
                case MaterialType.Oak:
                    return 10.0m; // Цена дуба
                case MaterialType.Pine:
                    return 8.0m;  // Цена сосны
                case MaterialType.Birch:
                    return 9.0m;  // Цена березы
                case MaterialType.Maple:
                    return 11.0m; // Цены клена
                default:
                    throw new FactoryException("Неизвестный тип материала.");
            }
        }
    }
}

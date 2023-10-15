using System.Collections.Generic;

namespace FactoryApp
{
    // Класс RawMaterialConfig представляет конфигурацию сырья на старте предприятия.
    public class RawMaterialConfig
    {
        public MaterialType Type { get; set; } // Тип сырья
        public int Quantity { get; set; }      // Начальное количество сырья данного типа
    }

    // Класс Configuration представляет конфигурацию предприятия на старте.
    public class Configuration
    {
        public decimal StartingBudget { get; set; } // Начальный бюджет предприятия
        public Manager? Manager { get; set; }        // Менеджер предприятия 
        public EmployeeCollection<Carpenter>? Carpenters { get; set; } // Коллекция столяров 
        public EmployeeCollection<Repairman>? Repairmen { get; set; } // Коллекция ремонтников 
        public List<Machine>? Machines { get; set; }  // Список станков 
        public List<RawMaterialConfig>? RawMaterials { get; set; } // Список конфигураций сырья 
    }
}

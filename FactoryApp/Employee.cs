using System.Xml.Serialization;

namespace FactoryApp
{
    // Перечисление, определяющее типы сотрудников
    public enum EmployeeType
    {
        Manager,    // Менеджер
        Carpenter,  // Столяр
        Repairman   // Ремонтник
    }

    // Абстрактный класс Employee
    public abstract class Employee
    {
        public int Id { get; set; } // Уникальный идентификатор сотрудника
        public EmployeeType Type { get; set; } // Тип сотрудника
        public string? Name { get; set; } // Имя сотрудника
        public decimal Salary { get; set; } // Зарплата сотрудника
        public bool IsBusy { get; set; } // Флаг, указывающий, занят ли сотрудник

        private static int nextId = 1; // Счетчик для генерации уникальных идентификаторов сотрудников

        // Конструктор класса Employee, инициализирующий экземпляр сотрудника с указанным именем и типом
        public Employee(string name, EmployeeType type)
        {
            Id = nextId++; // Присвоение уникального идентификатора сотруднику и инкремент счетчика
            Name = name; // Присвоение имени сотрудника
            Type = type; // Присвоение типа сотрудника
            IsBusy = false; // Установка флага "сотрудник не занят"
        }

        // Пустой конструктор для XML сериализации
        public Employee() { }

        // Абстрактный метод для расчета зарплаты сотрудника
        public abstract decimal CalculateSalary();

        // Переопределение метода ToString для получения информации о сотруднике в виде строки
        public override string ToString()
        {
            return $"ID: {Id}, Имя: {Name}, Позиция: {Type}, Зарплата: {Salary}";
        }
    }
}

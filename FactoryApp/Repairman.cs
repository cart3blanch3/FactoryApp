using System;
using System.Threading.Tasks;

namespace FactoryApp
{
    // Класс Repairman (Ремонтник), наследующийся от Employee
    public class Repairman : Employee
    {
        // Добавьте свойство для отслеживания количества починенных станков
        public int RepairedMachineCount { get; set; } = 0;
        public new decimal Salary => CalculateSalary();

        // Конструктор 
        public Repairman(string name) : base(name, EmployeeType.Repairman)
        {
        }

        // Пустой конструктор для XML сериализации
        public Repairman() : base(string.Empty, EmployeeType.Repairman)
        {
        }

        // Переопределение метода расчёта зарплаты на основе количества починенных станков
        public override decimal CalculateSalary()
        {
            return 1000 + RepairedMachineCount * 110.7m;
        }

        // Метод для ремонта сломанных машин
        public async Task RepairMachineAsync(Machine machine)
        {
            // Проверка, что станок не равен null и сломан
            if (machine != null && machine.IsBroken)
            {
                Logger.Log($"Ремонт станка {machine.Id}..."); // Логгирование начала ремонта
                await Task.Delay(machine.RepairTimeMilliseconds); // Асинхронная задержка для имитации времени починки
                machine.Repair(); // Вызов метода починки станка
                Logger.Log($"Станок {machine.Id} был отремонтирован."); // Логгирование окончания ремонта
                RepairedMachineCount++; // Увеличение счетчика отремонтированных станков
            }
            else
            {
                throw new ArgumentNullException(nameof(machine));
            }
        }
    }
}

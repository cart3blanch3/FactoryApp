namespace FactoryApp
{
    // Класс Manager (Менеджер), наследующийся от Employee
    public class Manager : Employee
    {
        // Переопределенное свойство Salary, вычисляющее зарплату
        public new decimal Salary => CalculateSalary();

        // Конструктор класса Manager
        public Manager(string name) : base(name, EmployeeType.Manager)
        {
        }

        // Переопределение метода расчёта зарплаты
        public override decimal CalculateSalary()
        {
            return 5000;
        }

        // Метод для обработки получения заказа
        public async Task HandleOrderReceived(Enterprise enterprise)
        {
            Order order = enterprise.GetOrderFromQueue(); // Получение заказа из очереди заказов

            while (true)
            {
                // Поиск свободного рабочего
                Carpenter carpenter = (Carpenter)enterprise.GetEmployees().FindAvailableEmployee(EmployeeType.Carpenter);

                if (carpenter != null)
                {
                    // Логгирование выдачи приказа столяру на производство
                    Logger.Log($"Получен заказ. Выдача приказа столяру {carpenter.Id} на производство {order.Quantity} {order.FurnitureType} из {order.MaterialType}.");

                    // Отдаем приказ столяру на выполнение заказа
                    await carpenter.ProduceFurnitureAsync(order, enterprise);

                    break; // Выход из цикла, так как нашли доступного работника
                }

                // Если не нашли доступного работника, подождем некоторое время и повторим проверку
                await Task.Delay(5000);
            }
        }

        // Метод для обработки поломки станка
        public async Task HandleMachineBroken(Machine machine, Enterprise enterprise)
        {
            while (true)
            {
                // Поиск свободного ремонтника для починки станка
                Repairman repairman = (Repairman)enterprise.GetEmployees().FindAvailableEmployee(EmployeeType.Repairman);

                if (repairman != null)
                {
                    // Логгирование выдачи приказа ремонтнику на починку станка
                    Logger.Log($"Станок сломан. Выдача приказа ремонтнику на починку станка.");

                    // Отдаем приказ ремонтнику на починку станка
                    repairman.RepairMachineAsync(machine).Wait(); // Ждем завершения ремонта

                    break; // Выход из цикла, так как нашли доступного ремонтника
                }

                // Если не нашли доступного ремонтника, подождем некоторое время и повторим проверку
                await Task.Delay(5000);
            }
        }

        // Метод для обработки недостаточного количества ресурсов
        public void HandleResoursesDepleted(MaterialType materialType, int quantity, Enterprise enterprise)
        {
            // Высчитываем цену закупки
            decimal materialPricePerUnit = Material.CalculatePricePerUnit(materialType);
            decimal totalMaterialCost = materialPricePerUnit * quantity;

            if (enterprise.CurrentBudget >= totalMaterialCost)
            {
                // Логгирование закупки сырья
                Logger.Log($"Закупка сырья. Тип: {materialType}, количество: {quantity}, стоимость: {totalMaterialCost}.");

                // Уменьшение бюджета предприятия на сумму закупки
                enterprise.AddExpenses(totalMaterialCost);

                Material material = enterprise.GetWarehouse().GetMaterialByType(materialType);

                if (material != null)
                {
                    // Добавление сырья на склад
                    enterprise.GetWarehouse().AddRawMaterial(material, quantity);
                }
                else
                {
                    throw new FactoryException("Неизвестный тип сырья");
                }
            }
            else
            {
                Logger.Log("Недостаточно средств для закупки материалов.");
            }
        }

        // Метод для обработки завершения производства
        public void HandleProductionCompleted(Order order, Enterprise enterprise)
        {
            // Получаем материал по его типу
            var furnitureMaterial = enterprise.GetWarehouse().GetMaterialByType(order.MaterialType);

            // Удаление готовых изделий, связанных с заказом, со склада
            Furniture furnitureToRemove = new Furniture(order.FurnitureType, furnitureMaterial);
            enterprise.GetWarehouse().RemoveFinishedProduct(furnitureToRemove, order.Quantity);

            // Расчет прибыли за заказ
            decimal totalProfit = order.TotalPrice;

            // Добавление прибыли в Enterprise
            enterprise.AddIncome(totalProfit);

            // Логгирование завершения заказа
            Logger.Log($"Заказ {order.Quantity} {order.FurnitureType} из {order.MaterialType} завершен. Получено: {totalProfit}ед. прибыли");
        }
    }
}

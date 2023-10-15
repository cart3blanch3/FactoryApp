using System;
using FactoryApp;
using NUnit.Framework;

namespace FactoryApp.Tests
{
    [TestFixture]
    public class EnterpriseTests
    {
        private Enterprise enterprise;

        [SetUp]
        public void SetUp()
        {
            // Создаем новый экземпляр предприятия перед каждым тестом
            enterprise = Enterprise.GetInstance(10000);
        }

        [TearDown]
        public void TearDown()
        {
            // Очищаем ресурсы после каждого теста

            // Удаляем менеджера, если он был добавлен
            enterprise.RemoveManager();

            // Очищаем очередь заказов
            enterprise.GetOrders().Clear();

            // Очищаем список сотрудников
            enterprise.GetEmployees().Clear();

            // Очищаем список станков
            enterprise.GetMachines().Clear();
        }

        [Test]
        public void AddManager_SetsManager()
        {
            // Arrange
            var manager = new Manager("John");

            // Act
            enterprise.AddManager(manager);

            // Assert
            // Проверяем, что менеджер был успешно добавлен и можно его найти
            Assert.AreEqual(manager, enterprise.GetEmployees().FindAvailableEmployee(EmployeeType.Manager));
        }

        [Test]
        public void AddManager_ThrowsExceptionWhenManagerExists()
        {
            // Arrange
            var manager1 = new Manager("John");
            var manager2 = new Manager("Sara");

            // Act
            enterprise.AddManager(manager1);

            // Assert
            // Проверяем, что попытка добавить второго менеджера вызывает исключение
            Assert.Throws<FactoryException>(() => enterprise.AddManager(manager2));
        }

        [Test]
        public void AddIncome_IncreasesIncome()
        {
            // Arrange
            var initialIncome = enterprise.Income;
            var amount = 500;

            // Act
            enterprise.AddIncome(amount);

            // Assert
            // Проверяем, что доход увеличился на заданную сумму
            Assert.AreEqual(initialIncome + amount, enterprise.Income);
        }

        [Test]
        public void AddIncome_ThrowsExceptionWhenAmountIsNegative()
        {
            // Arrange
            var amount = -500;

            // Act & Assert
            // Проверяем, что попытка добавить отрицательный доход вызывает исключение
            Assert.Throws<FactoryException>(() => enterprise.AddIncome(amount));
        }

        [Test]
        public void AddExpenses_IncreasesExpenses()
        {
            // Arrange
            var initialExpenses = enterprise.Expenses;
            var amount = 500;

            // Act
            enterprise.AddExpenses(amount);

            // Assert
            // Проверяем, что расходы увеличились на заданную сумму
            Assert.AreEqual(initialExpenses + amount, enterprise.Expenses);
        }

        [Test]
        public void AddExpenses_ThrowsExceptionWhenAmountIsNegative()
        {
            // Arrange
            var amount = -500;

            // Act & Assert
            // Проверяем, что попытка добавить отрицательные расходы вызывает исключение
            Assert.Throws<FactoryException>(() => enterprise.AddExpenses(amount));
        }

        [Test]
        public void EnqueueOrder_AddsOrderToQueue()
        {
            // Arrange
            var order = new Order(FurnitureType.Chair, MaterialType.Birch, 5);

            // Act
            enterprise.EnqueueOrder(order);

            // Assert
            // Проверяем, что заказ успешно добавлен в очередь
            var queue = enterprise.GetOrders();
            Assert.AreEqual(1, queue.Count);
            Assert.AreEqual(order, queue.Peek());
        }

        [Test]
        public void EnqueueOrder_ThrowsExceptionWhenOrderIsNull()
        {
            // Arrange
            Order order = null;

            // Act & Assert
            // Проверяем, что попытка добавить null-заказ вызывает исключение
            Assert.Throws<FactoryException>(() => enterprise.EnqueueOrder(order));
        }

        [Test]
        public void GetOrderFromQueue_GetsOrderFromQueue()
        {
            // Arrange
            var order1 = new Order(FurnitureType.Table, MaterialType.Oak, 3);
            var order2 = new Order(FurnitureType.Wardrobe, MaterialType.Pine, 2);
            enterprise.EnqueueOrder(order1);
            enterprise.EnqueueOrder(order2);

            // Act
            var retrievedOrder1 = enterprise.GetOrderFromQueue();
            var retrievedOrder2 = enterprise.GetOrderFromQueue();

            // Assert
            // Проверяем, что заказы извлекаются из очереди в правильной последовательности
            Assert.AreEqual(order1, retrievedOrder1);
            Assert.AreEqual(order2, retrievedOrder2);
        }

        [Test]
        public void GetOrderFromQueue_ThrowsExceptionWhenQueueIsEmpty()
        {
            // Act & Assert
            // Проверяем, что попытка извлечь заказ из пустой очереди вызывает исключение
            Assert.Throws<FactoryException>(() => enterprise.GetOrderFromQueue());
        }

        [Test]
        public void AddEmployee_AddsEmployeeToCollection()
        {
            // Arrange
            var carpenter = new Carpenter("Alice");

            // Act
            enterprise.AddEmployee(carpenter);

            // Assert
            // Проверяем, что работник успешно добавлен в коллекцию
            var employees = enterprise.GetEmployees();
            Assert.AreEqual(1, employees.Count);
            Assert.IsTrue(employees.Contains(carpenter));
        }

        [Test]
        public void AddEmployee_ThrowsExceptionWhenEmployeeIsNull()
        {
            // Arrange
            Employee employee = null;

            // Act & Assert
            // Проверяем, что попытка добавить null-работника вызывает исключение
            Assert.Throws<FactoryException>(() => enterprise.AddEmployee(employee));
        }

        [Test]
        public void AddMachine_AddsMachineToList()
        {
            // Arrange
            var machine = new Machine(100, 500);

            // Act
            enterprise.AddMachine(machine);

            // Assert
            // Проверяем, что станок успешно добавлен в список
            var machines = enterprise.GetMachines();
            Assert.AreEqual(1, machines.Count);
            Assert.AreEqual(machine, machines[0]);
        }

        [Test]
        public void GetCurrentConfiguration_ReturnsCurrentConfiguration()
        {
            // Arrange
            var manager = new Manager("John");
            enterprise.AddManager(manager);

            var carpenter = new Carpenter("Alice");
            enterprise.AddEmployee(carpenter);

            var machine = new Machine(100, 500);
            enterprise.AddMachine(machine);

            var order = new Order(FurnitureType.Chair, MaterialType.Birch, 5);
            enterprise.EnqueueOrder(order);

            enterprise.AddIncome(1000);
            enterprise.AddExpenses(300);

            // Добавляем материалы на склад
            var birchMaterial = new Material(MaterialType.Birch);
            enterprise.GetWarehouse().AddRawMaterial(birchMaterial, 5);

            // Act
            var config = enterprise.GetCurrentConfiguration();

            // Assert
            // Проверяем, что текущая конфигурация предприятия включает в себя заданных сотрудников, станок и материалы
            Assert.AreEqual(manager, config.Manager);
            Assert.AreEqual(1, config.Carpenters.Count);
            Assert.IsTrue(config.Carpenters.Contains(carpenter));
            Assert.AreEqual(1, config.Machines.Count);
            Assert.AreEqual(machine.MaxDurability, config.Machines[0].MaxDurability);
            Assert.AreEqual(machine.RepairTimeMilliseconds, config.Machines[0].RepairTimeMilliseconds);
            Assert.AreEqual(1, config.RawMaterials.Count);
            Assert.AreEqual(MaterialType.Birch, config.RawMaterials[0].Type);
            Assert.AreEqual(5, config.RawMaterials[0].Quantity);
        }
    }
}

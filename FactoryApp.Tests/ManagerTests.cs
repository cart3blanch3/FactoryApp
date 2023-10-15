using FactoryApp;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class ManagerTests
    {
        private Manager manager;
        private Enterprise enterprise;
        private Warehouse warehouse;

        [SetUp]
        public void SetUp()
        {
            // Создаем экземпляры менеджера, предприятия и склада перед каждым тестом
            manager = new Manager("Alice");
            enterprise = Enterprise.GetInstance(10000);
            warehouse = enterprise.GetWarehouse();

            // Инициализация склада
            warehouse.AddRawMaterial(new Material(MaterialType.Pine), 100);
            warehouse.AddRawMaterial(new Material(MaterialType.Birch), 0);
            warehouse.AddRawMaterial(new Material(MaterialType.Maple), 0);
            warehouse.AddRawMaterial(new Material(MaterialType.Oak), 0);
        }

        [TearDown]
        public void TearDown()
        {
            // Освобождаем ресурсы после каждого теста
            warehouse.RawMaterials.Clear();
            warehouse.FinishedProducts.Clear();
        }

        [Test]
        public void CalculateSalary_ReturnsFixedSalary()
        {
            // Act (Действие)
            decimal salary = manager.CalculateSalary(); // Вычисляем зарплату менеджера

            // Assert (Проверка)
            Assert.AreEqual(5000m, salary); // Утверждаем, что зарплата равна ожидаемому значению (5000)
        }

        [Test]
        public void HandleResourcesDepleted_EnoughBudget_AddsMaterialsToWarehouseAndReducesBudget()
        {
            // Arrange (Подготовка данных)
            // Добавляем некоторое количество материала на склад и фиксируем начальный бюджет предприятия
            var initialBudget = enterprise.CurrentBudget;

            // Act (Действие)
            manager.HandleResoursesDepleted(MaterialType.Oak, 5, enterprise); // Обрабатываем ситуацию, когда есть достаточно бюджета

            // Assert (Проверка)
            // Проверяем, что материал добавлен на склад и бюджет уменьшен на соответствующую сумму
            var material = warehouse.GetMaterialByType(MaterialType.Oak);
            Assert.IsNotNull(material); // Утверждаем, что материал добавлен
            Assert.AreEqual(5, warehouse.RawMaterials[material]); // Утверждаем, что количество материала на складе увеличено
            Assert.AreEqual(initialBudget - 5 * Material.CalculatePricePerUnit(MaterialType.Oak), enterprise.CurrentBudget); // Утверждаем, что бюджет уменьшен
        }

        [Test]
        public void HandleResourcesDepleted_InsufficientBudget_DoesNotAddMaterialsToWarehouse()
        {
            // Arrange (Подготовка данных)
            // Фиксируем начальный бюджет предприятия, который недостаточен для закупки материала
            enterprise.AddExpenses(10000); // Делаем бюджет равный нулю
            var material = warehouse.GetMaterialByType(MaterialType.Birch);
            var initialBudget = enterprise.CurrentBudget;
            var materialCount = warehouse.RawMaterials[material];

            // Act (Действие)
            manager.HandleResoursesDepleted(MaterialType.Birch, 10, enterprise); // Обрабатываем ситуацию, когда бюджет недостаточен

            // Assert (Проверка)
            Assert.AreEqual(materialCount, warehouse.RawMaterials[material]); // Утверждаем, что количество материала на складе не изменилось
            Assert.AreEqual(initialBudget, enterprise.CurrentBudget); // Утверждаем, что бюджет не изменился
        }

        [Test]
        public void HandleProductionCompleted_RemovesFinishedProducts_AddsIncome()
        {
            // Arrange (Подготовка данных)
            // Создаем материал и мебель на складе, а также заказ и фиксируем начальный бюджет предприятия
            var material = warehouse.GetMaterialByType(MaterialType.Pine);
            var furniture = new Furniture(FurnitureType.Table, material);
            warehouse.AddFinishedProduct(furniture, 5);
            var order = new Order(FurnitureType.Table, MaterialType.Pine, 5);
            var initialBudget = enterprise.CurrentBudget;

            // Act (Действие)
            manager.HandleProductionCompleted(order, enterprise); // Обрабатываем завершение заказа

            // Assert (Проверка)
            // Проверяем, что мебель удалена со склада и бюджет увеличен на ожидаемую сумму
            var product = warehouse.FinishedProducts.FirstOrDefault(item => item.Key == furniture);
            Assert.AreEqual(0, product.Value); // Утверждаем, что мебель удалена со склада
            var expectedIncome = order.TotalPrice; // Ожидаемая прибыль за заказ
            Assert.AreEqual(initialBudget + expectedIncome, enterprise.CurrentBudget); // Утверждаем, что бюджет увеличен на ожидаемую сумму
        }

        [Test]
        public void HandleOrderReceived_AllocatesOrderToAvailableCarpenter()
        {
            // Arrange (Подготовка данных)
            // Создаем заказ и добавляем столяра в список работников предприятия
            var order = new Order(FurnitureType.Table, MaterialType.Maple, 2);
            var carpenter = new Carpenter("John");
            enterprise.AddEmployee(carpenter);
            enterprise.EnqueueOrder(order);

            // Act (Действие)
            manager.HandleOrderReceived(enterprise); // Обрабатываем получение заказа

            // Assert (Проверка)
            // Проверяем, что заказ удален из очереди и столяр помечен как "занят"
            Assert.IsFalse(enterprise.GetOrders().Contains(order)); // Утверждаем, что заказ удален из очереди
            Assert.IsTrue(carpenter.IsBusy); // Утверждаем, что столяр помечен как "занят"
        }
    }
}

using NUnit.Framework;
using System.Threading.Tasks;
using FactoryApp;

namespace Tests
{
    [TestFixture]
    public class CarpenterTests
    {
        private Enterprise enterprise;
        private Carpenter carpenter;
        private Warehouse warehouse;

        [SetUp]
        public void SetUp()
        {
            // Создаем предприятие, столяра и склад для каждого теста
            // Создание экземпляра класса Enterprise с начальным бюджетом 10000
            enterprise = Enterprise.GetInstance(10000);
            carpenter = new Carpenter("John Doe");
            warehouse = enterprise.GetWarehouse();
        }

        [TearDown]
        public void TearDown()
        {
            // Освобождаем ресурсы после каждого теста
            enterprise = null;
            carpenter = null;
            warehouse = null;
        }

        [Test]
        public void CalculateSalary_WithZeroProducedItems_ReturnsBaseSalary()
        {
            // Утверждаем, что зарплата столяра с нулевым количеством произведенных изделий равна базовой зарплате.
            Assert.AreEqual(1000m, carpenter.CalculateSalary());
        }

        [Test]
        public void CalculateSalary_WithProducedItems_ReturnsCorrectSalary()
        {
            // Устанавливаем количество произведенных изделий и проверяем, что зарплата правильно рассчитана.
            carpenter.ProducedItemCount = 10;
            Assert.AreEqual(2255.5m, carpenter.CalculateSalary());
        }

        [Test]
        public async Task ProduceFurnitureAsync_CompletesOrder_CreatesFurniture()
        {
            // Создаем заказ на 5 столов из березы
            var order = new Order(FurnitureType.Table, MaterialType.Birch, 5);

            // Вызываем метод для производства
            await carpenter.ProduceFurnitureAsync(order, enterprise);

            // Проверяем, что на складе появились 5 столов
            var finishedFurniture = warehouse.FinishedProducts.FirstOrDefault(product => product.Key.Type == FurnitureType.Table);
            Assert.AreEqual(5, finishedFurniture.Value);
        }

        [Test]
        public async Task ProduceFurnitureAsync_InsufficientMaterial_WaitsForMaterial()
        {
            // Создаем заказ на 10 столов из дуба
            var order = new Order(FurnitureType.Table, MaterialType.Oak, 10);

            // Вызываем метод для производства
            var task = carpenter.ProduceFurnitureAsync(order, enterprise);

            // Ждем некоторое время, чтобы позволить столяру ожидать материал
            await Task.Delay(2000);

            // Проверяем, что столяр ждет материал
            Assert.IsTrue(task.Status == TaskStatus.WaitingForActivation);

            // Добавляем необходимое количество материала на склад
            warehouse.AddRawMaterial(new Material(MaterialType.Oak), 100);

            // Ждем завершения задачи
            await task;

            // Проверяем, что на складе появились 10 столов
            var finishedFurniture = warehouse.FinishedProducts.FirstOrDefault(product => product.Key.Type == FurnitureType.Table);
            Assert.AreEqual(10, finishedFurniture.Value);
        }
    }
}

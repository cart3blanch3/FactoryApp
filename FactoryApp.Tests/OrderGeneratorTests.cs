using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FactoryApp;
using NUnit.Framework;

namespace FactoryApp.Tests
{
    [TestFixture]
    public class OrderGeneratorTests
    {
        private OrderGenerator orderGenerator;
        private Enterprise enterprise;

        [SetUp]
        public void SetUp()
        {
            // Создать экземпляр OrderGenerator и Enterprise перед каждым тестом
            orderGenerator = new OrderGenerator();
            enterprise = Enterprise.GetInstance(10000);
        }

        [TearDown]
        public void TearDown()
        {
            // Освободить ресурсы после каждого теста
            orderGenerator = null;
            enterprise = null;
        }

        [Test]
        public async Task GenerateRandomOrdersAsync_GeneratesOrders()
        {
            // Act
            var count = 5;
            await orderGenerator.GenerateRandomOrdersAsync(count, enterprise);

            // Assert
            var orders = enterprise.GetOrders();
            Assert.AreEqual(count, orders.Count);
        }

        [Test]
        public async Task GenerateRandomOrdersAsync_ValidOrders()
        {
            // Act
            var count = 5;
            await orderGenerator.GenerateRandomOrdersAsync(count, enterprise);

            // Assert
            var orders = enterprise.GetOrders();
            foreach (var order in orders)
            {
                Assert.IsInstanceOf<FurnitureType>(order.FurnitureType);
                Assert.IsInstanceOf<MaterialType>(order.MaterialType);
                Assert.IsTrue(order.Quantity >= 1 && order.Quantity <= 10);
            }
        }
    }
}

using System;
using System.Threading.Tasks;
using FactoryApp;
using NUnit.Framework;

namespace FactoryApp.Tests
{
    [TestFixture]
    public class RepairmanTests
    {
        private Repairman repairman;
        private Machine brokenMachine;

        [SetUp]
        public void SetUp()
        {
            repairman = new Repairman("John");
            brokenMachine = new Machine(1, 1000);
            brokenMachine.Use(); // Ломаем станок
        }

        [TearDown]
        public void TearDown()
        {
            repairman = null;
            brokenMachine = null;
        }

        [Test]
        public void CalculateSalary_ReturnsCorrectSalary()
        {
            // Arrange (Подготовка данных)
            repairman.RepairedMachineCount = 5; // Устанавливаем количество отремонтированных станков

            // Act (Действие)
            decimal salary = repairman.CalculateSalary(); // Вычисляем зарплату ремонтника

            // Assert (Проверка)
            decimal expectedSalary = 1000 + 5 * 110.7m; // Ожидаемая зарплата
            Assert.AreEqual(expectedSalary, salary); // Проверяем, что зарплата соответствует ожидаемой сумме
        }

        [Test]
        public async Task RepairMachineAsync_RepairBrokenMachine()
        {
            // Act (Действие)
            await repairman.RepairMachineAsync(brokenMachine); // Ремонтируем сломанный станок

            // Assert (Проверка)
            Assert.IsFalse(brokenMachine.IsBroken); // Проверяем, что станок был успешно отремонтирован
            Assert.AreEqual(1, repairman.RepairedMachineCount); // Проверяем, что счетчик отремонтированных станков увеличен на 1
        }

        [Test]
        public async Task RepairMachineAsync_NullMachine_ThrowsArgumentNullException()
        {
            // Arrange (Подготовка данных)
            Machine nullMachine = null; // Станок равен null

            // Act & Assert (Действие и проверка)
            var exception = Assert.ThrowsAsync<ArgumentNullException>(() => repairman.RepairMachineAsync(nullMachine));
            // Проверяем, что при попытке ремонта null станка выбрасывается исключение ArgumentNullException
        }
    }
}

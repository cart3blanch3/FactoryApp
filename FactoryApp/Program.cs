using System;
using System.IO;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace FactoryApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Создание и настройка логгера
            Logger logger = Logger.Instance;
            string logFilePath = $"log.txt"; // Уникальный путь к файлу для каждого потока
            
            // Создание экземпляра класса ConsoleFilePrinter для вывода логов как в консоль и файл
            ConsoleFilePrinter consoleFilePrinter = new ConsoleFilePrinter(logFilePath);

            // Подписываемся на событие LogEvent логгера, чтобы выводить логи в консоль и файл
            logger.LogEvent += consoleFilePrinter.PrintToConsoleAndFile;

            try
            {
                // Задание пути к файлу конфигурации
                string configFilePath = "config.json"; // Путь к файлу конфигурации

                // Загрузка конфигурации из файла
                Configuration configuration = ConfigurationLoader.LoadConfiguration(configFilePath);

                if (configuration != null)
                {
                    // Создание предприятия с начальным бюджетом из конфигурации
                    Enterprise enterprise = Enterprise.GetInstance(configuration.StartingBudget);

                    // Запуск работы предприятия
                    enterprise.RunEnterprise();

                    // Инициализация данных из конфигурации
                    InitializeDataFromConfiguration(enterprise, configuration);

                    // Создание генератора заказов
                    OrderGenerator orderGenerator = new OrderGenerator();

                    // Основной код для выполнения в основном потоке
                    while (true)
                    {
                        // Генерация случайных заказов и добавление их в предприятие
                        await orderGenerator.GenerateRandomOrdersAsync(5, enterprise);
                        await Task.Delay(5000); // Пауза перед генерацией новых заказов

                        // Завершение работы при достижении определенного бюджета
                        if (enterprise.CurrentBudget >= 10000)
                        {
                            Console.WriteLine("Достигнут заданный бюджет. Завершение работы предприятия.");

                            // Создание коллекций работников и сортировка их по зарплате
                            var carpenters = new EmployeeCollection<Carpenter>(enterprise.GetEmployees().OfType<Carpenter>());
                            carpenters.SortingStarted += (message) => Console.WriteLine(message);
                            carpenters.SortingCompleted += (count) => Console.WriteLine($"Сортировка завершена. Отсортировано {count} элементов.");
                            await carpenters.SortAsync(employees => employees.Sort((x, y) => x.Salary.CompareTo(y.Salary)));

                            var repairmen = new EmployeeCollection<Repairman>(enterprise.GetEmployees().OfType<Repairman>());
                            repairmen.SortingStarted += (message) => Console.WriteLine(message);
                            repairmen.SortingCompleted += (count) => Console.WriteLine($"Сортировка завершена. Отсортировано {count} элементов.");
                            await repairmen.SortAsync(employees => employees.Sort((x, y) => x.Salary.CompareTo(y.Salary)));

                            // Сериализация данных о работниках в XML-формате
                            var serializer_xml_1 = new XmlDataSerializer<EmployeeCollection<Carpenter>>();
                            serializer_xml_1.Serialize("carpenter_data.xml", carpenters);
                            var serializer_xml_2 = new XmlDataSerializer<EmployeeCollection<Repairman>>();
                            serializer_xml_2.Serialize("repairmen_data.xml", repairmen);

                            // Сериализация данных о работниках в JSON-формате
                            var serializer_json_1 = new JsonDataSerializer<EmployeeCollection<Carpenter>>();
                            serializer_json_1.Serialize("carpenter_data.json", carpenters);
                            var serializer_json_2 = new JsonDataSerializer<EmployeeCollection<Repairman>>();
                            serializer_json_2.Serialize("repairmen_data.xml", repairmen);

                            // Получение текущей конфигурации предприятия и сохранение её в файл
                            Configuration currentConfiguration = enterprise.GetCurrentConfiguration();
                            ConfigurationLoader.SaveConfiguration(currentConfiguration, configFilePath);

                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Обработка исключений и логирование ошибок
                Logger.Log($"{ex.Message}");
            }
        }

        // Метод для инициализации данных из конфигурации
        private static void InitializeDataFromConfiguration(Enterprise enterprise, Configuration configuration)
        {
            // Добавление менеджера в предприятие
            enterprise.AddManager(configuration.Manager);

            // Добавление столяров в предприятие
            foreach (var carpenterConfig in configuration.Carpenters)
            {
                carpenterConfig.IsBusy = false;
                enterprise.AddEmployee(carpenterConfig);
            }

            // Добавление ремонтников в предприятие
            foreach (var repairman in configuration.Repairmen)
            {
                repairman.IsBusy = false;
                enterprise.AddEmployee(repairman);
            }

            // Добавление станков в предприятие
            foreach (var machineConfig in configuration.Machines)
            {
                enterprise.AddMachine(machineConfig);
            }

            // Добавление сырья на склад предприятия
            foreach (var materialConfig in configuration.RawMaterials)
            {
                Material material = new Material(materialConfig.Type);
                enterprise.GetWarehouse().AddRawMaterial(material, materialConfig.Quantity);
            }
        }
    }
}

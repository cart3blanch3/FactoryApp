namespace FactoryApp
{
    // Класс предприятия
    public class Enterprise
    {
        // Событие, которое вызывается при получении нового заказа
        public event Action<Order>? OrderReceived;
        // Событие, которое вызывается при добавлении столяра
        public event Action<Carpenter>? CarpenterAdded;
        // Событие, которое вызывается при добавлении станка
        public event Action<Machine>? MachineAdded;


        // Приватное статическое поле для хранения единственного экземпляра класса Enterprise
        private static Enterprise? instance;

        // Приватное поле для хранения менеджера
        private Manager manager;

        // Коллекция работников предприятия
        private EmployeeCollection<Employee> employees;

        // Список станков на предприятии
        private List<Machine> machines;

        // Склад предприятия
        private Warehouse warehouse;

        // Очередь заказов
        private Queue<Order> orderQueue = new Queue<Order>();

        // Свойство для начального бюджета предприятия
        public decimal StartingBudget { get; private set; }

        // Свойство для хранения доходов предприятия
        public decimal Income { get; private set; }

        // Свойство для хранения расходов предприятия
        public decimal Expenses { get; private set; }

        // Свойство для текущего бюджета предприятия (рассчитывается как начальный бюджет плюс доходы минус расходы)
        public decimal CurrentBudget => StartingBudget + Income - Expenses;

        // Приватный конструктор для создания экземпляра класса Enterprise
        private Enterprise(decimal startingBudget)
        {
            employees = new EmployeeCollection<Employee>();
            machines = new List<Machine>();
            warehouse = new Warehouse();
            StartingBudget = startingBudget;
        }

        // Метод для получения единственного экземпляра класса Enterprise
        public static Enterprise GetInstance(decimal startingBudget)
        {
            if (instance == null)
            {
                instance = new Enterprise(startingBudget);
            }
            return instance;
        }

        // Добавление менеджера в предприятие
        public void AddManager(Manager newManager)
        {
            if (manager == null)
            {
                manager = newManager;
                AddEmployee(manager); // Добавляем менеджера в список сотрудников
            }
            else
            {
                throw new FactoryException("Менеджер уже существует. Менеджер может быть только один.");
            }
        }

        //Метод удаление менеджера
        public void RemoveManager()
        {
            manager = null;
            var managerToRemove = employees.FindAvailableEmployee(EmployeeType.Manager);
            employees.Remove(managerToRemove);
        }


        // Метод для учета доходов
        public void AddIncome(decimal amount)
        {
            if (amount < 0)
            {
                throw new FactoryException("Сумма дохода не может быть отрицательной.", new ArgumentException(nameof(amount)));
            }
            Income += amount;
        }

        // Метод для учета расходов
        public void AddExpenses(decimal amount)
        {
            if (amount < 0)
            {
                throw new FactoryException("Сумма расхода не может быть отрицательной.", new ArgumentException(nameof(amount)));
            }
            Expenses += amount;
        }

        // Метод для добавления заказа в очередь
        public void EnqueueOrder(Order order)
        {
            if (order == null)
            {
                throw new FactoryException("Заказ не может быть null.", new ArgumentNullException(nameof(order)));
            }
            orderQueue.Enqueue(order);
            OrderReceived?.Invoke(order);
        }

        // Метод для получения заказа из очереди
        public Order? GetOrderFromQueue()
        {
            if (orderQueue.Count == 0)
            {
                throw new FactoryException("Очередь заказов пуста.", new InvalidOperationException());
            }
            return orderQueue.Dequeue();
        }

        // Метод для добавления работника в предприятие
        public void AddEmployee(Employee employee)
        {
            if (employee == null)
            {
                throw new FactoryException("Работник не может быть null.", new ArgumentNullException(nameof(employee)));
            }
            employees.Add(employee);

            // Если добавленный работник - столяр, вызываем событие
            if (employee.Type == EmployeeType.Carpenter)
            {
                CarpenterAdded?.Invoke((Carpenter)employee);
            }
        }

        // Метод для добавления станка в предприятие
        public void AddMachine(Machine machine)
        {
            machines.Add(machine);
            MachineAdded?.Invoke(machine);
        }

        // Метод для получения списка работников предприятия
        public EmployeeCollection<Employee> GetEmployees()
        {
            return employees;
        }

        // Метод для получения списка станков на предприятии
        public List<Machine> GetMachines()
        {
            return machines;
        }

        // Метод для получения склада предприятия
        public Warehouse GetWarehouse()
        {
            return warehouse;
        }

        // Мето для получения очереди заказов предприятия
        public Queue<Order> GetOrders() 
        {
            return orderQueue;
        }

        // Метод получения текущей конфигурации предприятия на основе текущего состояния всех его компонентов.
        public Configuration GetCurrentConfiguration()
        {
            // Создаем новый объект конфигурации, который будет содержать текущую конфигурацию предприятия.
            var currentConfiguration = new Configuration
            {
                StartingBudget = CurrentBudget, // Устанавливаем начальный бюджет из текущего состояния бюджета.

                Manager = null, // Здесь нужно установить текущего менеджера, если он существует. В начале устанавливаем значение null.

                Carpenters = new EmployeeCollection<Carpenter>(), // Создаем коллекцию столяров для текущей конфигурации.

                Repairmen = new EmployeeCollection<Repairman>(), // Создаем коллекцию ремонтников для текущей конфигурации.

                Machines = new List<Machine>(), // Создаем пустой список для станков.

                RawMaterials = new List<RawMaterialConfig>(), // Создаем пустой список для информации о сырье.
            };

            // Добавляем информацию о сырье на складе в текущую конфигурацию.
            var warehouse = GetWarehouse();
            foreach (var material in warehouse.RawMaterials.Keys)
            {
                currentConfiguration.RawMaterials.Add(new RawMaterialConfig
                {
                    Type = material.Type, // Устанавливаем тип сырья.
                    Quantity = warehouse.RawMaterials[material] // Устанавливаем количество сырья.
                });
            }

            // Заполняем Manager, Carpenters и Repairmen на основе текущих сотрудников предприятия.
            var employees = GetEmployees();
            foreach (var employee in employees)
            {
                if (employee is Manager manager)
                {
                    currentConfiguration.Manager = manager; // Если сотрудник является менеджером, то устанавливаем его как текущего менеджера.
                }
                else if (employee is Carpenter carpenter)
                {
                    currentConfiguration.Carpenters.Add(carpenter); // Если сотрудник является столяром, то добавляем его в коллекцию столяров.
                }
                else if (employee is Repairman repairman)
                {
                    currentConfiguration.Repairmen.Add(repairman); // Если сотрудник является ремонтником, то добавляем его в коллекцию ремонтников.
                }
            }

            // Добавляем информацию о станках в текущую конфигурацию.
            foreach (var machine in GetMachines())
            {
                currentConfiguration.Machines.Add(new Machine(machine.MaxDurability, machine.RepairTimeMilliseconds)); // Создаем новые объекты станков на основе существующих станков.
            }

            return currentConfiguration; // Возвращаем сформированную текущую конфигурацию предприятия.
        }

        // Метод для запуска автономной работы предприятия
        public void RunEnterprise()
        {
            var warehouse = GetWarehouse();

            // Создание потока для менеджера
            Thread managerThread = new Thread(() =>
            {
                // Получение менеджера из списка работников (пока неизвестен)
                Manager manager = null;

                // Список задач, созданных внутри потока менеджера
                List<Task> managerTasks = new List<Task>();

                // Подписываем менеджера на событие поступления заказа
                OrderReceived += async (o) =>
                {
                    // Если менеджер еще не известен, то пытаемся найти его
                    if (manager == null)
                    {
                        manager = (Manager)GetEmployees().FindAvailableEmployee(EmployeeType.Manager);
                    }

                    // Если менеджер найден, обрабатываем заказ и добавляем задачу в список
                    if (manager != null)
                    {
                        managerTasks.Add(Task.Run(() => manager.HandleOrderReceived(this)));
                    }
                };

                // Подписываем менеджера на событие поломки станка
                MachineAdded += (m) => m.MachineBroken += async (m) =>
                {
                    // Если менеджер еще не известен, то пытаемся найти его
                    if (manager == null)
                    {
                        manager = (Manager)GetEmployees().FindAvailableEmployee(EmployeeType.Manager);
                    }

                    // Если менеджер найден, обрабатываем поломку станка и добавляем задачу в список
                    if (manager != null)
                    {
                        managerTasks.Add(Task.Run(() => manager.HandleMachineBroken(m, this)));
                    }
                };

                // Подписываем менеджера на событие завершения заказа
                CarpenterAdded += (c) => c.ProductionCompleted += async (o) =>
                {
                    // Если менеджер еще не известен, то пытаемся найти его
                    if (manager == null)
                    {
                        manager = (Manager)GetEmployees().FindAvailableEmployee(EmployeeType.Manager);
                    }

                    // Если менеджер найден, обрабатываем завершение заказа и добавляем задачу в список
                    if (manager != null)
                    {
                        managerTasks.Add(Task.Run(() => manager.HandleProductionCompleted(o, this)));
                    }
                };

                // Подписываем менеджера на событие нехватки ресурсов
                warehouse.ResourceDepleted += async (m) =>
                {
                    // Если менеджер еще не известен, то пытаемся найти его
                    if (manager == null)
                    {
                        manager = (Manager)GetEmployees().FindAvailableEmployee(EmployeeType.Manager);
                    }

                    // Если менеджер найден, обрабатываем нехватку ресурсов и добавляем задачу в список
                    if (manager != null)
                    {
                        managerTasks.Add(Task.Run(() => manager.HandleResoursesDepleted(m.Type, 100, this)));
                    }
                };

                // Дождитесь завершения всех задач менеджера
                Task.WhenAll(managerTasks).Wait();
            });

            // Создаем и запускаем поток
            managerThread.Start();

            // Ожидаем завершения потока менеджера
            managerThread.Join();
        }
    }
}

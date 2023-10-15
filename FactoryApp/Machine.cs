namespace FactoryApp
{
    // Класс станка
    public class Machine
    {
        public int Id { get; private set; } // Уникальный идентификатор станка
        public int MaxDurability { get; private set; } // Максимальная прочность станка
        public int CurrentDurability { get; private set; } // Текущая прочность станка
        public bool IsOccupied { get; private set; } // Проверка, занят ли станок
        public bool IsBroken { get; private set; } // Проверка, сломан ли станок
        public int RepairTimeMilliseconds { get; private set; } // Время починки станка в миллисекундах

        // Определение события для события поломки станка
        public event Action<Machine>? MachineBroken;

        // Метод для вызова события поломки станка
        private void OnMachineBroken()
        {
            MachineBroken?.Invoke(this);
        }

        private static int nextMachineId = 1;

        // Конструктор класса Machine
        public Machine(int maxDurability, int repairTimeMilliseconds)
        {
            Id = nextMachineId++; // Установка идентификатора станка
            MaxDurability = maxDurability; // Установка максимальной прочности станка
            CurrentDurability = maxDurability; // Изначально текущая прочность равна максимальной
            IsOccupied = false; // Изначально станок не занят
            IsBroken = false; // Изначально станок не сломан
            RepairTimeMilliseconds = repairTimeMilliseconds; // Установка времени починки станка
        }

        // Метод для использования станка
        public void Use()
        {
            if (CurrentDurability > 0 && !IsOccupied && !IsBroken)
            {
                IsOccupied = true; // Станок становится занятым
                CurrentDurability--;

                if (CurrentDurability == 0)
                {
                    IsBroken = true; // Станок считается поломанным, когда прочность достигает нуля
                    OnMachineBroken(); // Вызов события поломки станка
                }
            }
        }

        // Метод для починки станка
        public void Repair()
        {
            // Восстановление прочности станка
            CurrentDurability = MaxDurability;
            IsBroken = false;
        }

        // Метод для освобождения станка
        public void Release()
        {
            IsOccupied = false; // Станок становится свободным
        }
    }
}

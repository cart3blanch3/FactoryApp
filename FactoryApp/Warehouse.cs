using System;
using System.Collections.Generic;
using System.Linq;

namespace FactoryApp
{
    // Класс склада
    public class Warehouse
    {
        // Объект-замок для синхронизации доступа к складу
        private readonly object warehouseLock = new object();

        // Событие, которое вызывается, когда какой-то ресурс заканчивается
        public event Action<Material>? ResourceDepleted;

        // Словарь для хранения информации о сырье и его количестве
        public Dictionary<Material, int> RawMaterials { get; } = new Dictionary<Material, int>();

        // Словарь для хранения информации о готовых изделиях и их количестве
        public Dictionary<Furniture, int> FinishedProducts { get; } = new Dictionary<Furniture, int>();

        // Добавление сырья на склад
        public void AddRawMaterial(Material material, int quantity)
        {
            lock (warehouseLock)
            {
                if (RawMaterials.ContainsKey(material))
                {
                    RawMaterials[material] += quantity;
                }
                else
                {
                    RawMaterials[material] = quantity;
                }
            }
        }

        // Удаление сырья со склада
        public void RemoveRawMaterial(Material material, int quantity)
        {
            lock (warehouseLock)
            {
                if (RawMaterials.ContainsKey(material) && RawMaterials[material] >= quantity)
                {
                    RawMaterials[material] -= quantity;
                    if (RawMaterials[material] == 0)
                    {
                        Logger.Log($"{material.Type} закончился на складе.");
                        OnResourceDepleted(material);
                    }
                }
                else
                {
                    throw new FactoryException("Недостаточно сырья на складе.");
                }
            }
        }

        // Метод для вызова события ResourceDepleted
        protected virtual void OnResourceDepleted(Material material)
        {
            ResourceDepleted?.Invoke(material);
        }

        // Добавление готовых изделий на склад
        public void AddFinishedProduct(Furniture furniture, int quantity)
        {
            lock (warehouseLock)
            {
                if (FinishedProducts.ContainsKey(furniture))
                {
                    FinishedProducts[furniture] += quantity;
                }
                else
                {
                    FinishedProducts[furniture] = quantity;
                }
            }
        }

        // Удаление готовых изделий со склада
        public void RemoveFinishedProduct(Furniture furniture, int quantity)
        {
            lock (warehouseLock)
            {
                if (FinishedProducts.ContainsKey(furniture) && FinishedProducts[furniture] >= quantity)
                {
                    FinishedProducts[furniture] -= quantity;
                }
                else
                {
                    throw new FactoryException("Недостаточно готовых изделий на складе.");
                }
            }
        }

        // Метод для получения материала по его типу
        public Material GetMaterialByType(MaterialType materialType)
        {
            return RawMaterials.Keys.FirstOrDefault(material => material.Type == materialType);
        }

        // Метод для проверки наличия сырья на складе
        public bool HasEnoughRawMaterial(Material material, int quantity)
        {
            lock (warehouseLock)
            {
                if (RawMaterials.ContainsKey(material) && RawMaterials[material] >= quantity)
                {
                    return true;
                }
                OnResourceDepleted(material);
                return false;
            }
        }
    }
}

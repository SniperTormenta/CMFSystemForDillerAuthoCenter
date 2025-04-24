using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMFSystemForDillerAuthoCenter
{
    public static class DataStorage
    {
        public static CarData CarData { get; set; } = new CarData();
        public static DealData DealData { get; set; } = new DealData();

        public static void LoadCars()
        {
            try
            {
                string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cars.json");
                if (File.Exists(fullPath))
                {
                    string json = File.ReadAllText(fullPath);
                    CarData = JsonConvert.DeserializeObject<CarData>(json) ?? new CarData();
                    System.Diagnostics.Debug.WriteLine($"DataStorage LoadCars: Загружено {CarData?.Cars?.Count ?? 0} автомобилей.");
                }
                else
                {
                    CarData = new CarData();
                    SaveCars();
                    System.Diagnostics.Debug.WriteLine($"DataStorage LoadCars: Файл {fullPath} не найден, создан новый пустой CarData.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка при загрузке автомобилей: {ex.Message}");
                CarData = new CarData();
            }
        }

        public static void SaveCars()
        {
            try
            {
                string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cars.json");
                string json = JsonConvert.SerializeObject(CarData, Formatting.Indented);
                File.WriteAllText(fullPath, json);
                System.Diagnostics.Debug.WriteLine($"DataStorage SaveCars: Файл {fullPath} сохранён. Содержимое:\n{json}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка при сохранении автомобилей: {ex.Message}");
            }
        }

        public static void LoadDeals()
        {
            try
            {
                string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "deals.json");
                if (File.Exists(fullPath))
                {
                    string json = File.ReadAllText(fullPath);
                    DealData = JsonConvert.DeserializeObject<DealData>(json) ?? new DealData();
                    System.Diagnostics.Debug.WriteLine($"DataStorage LoadDeals: Загружено {DealData?.Deals?.Count ?? 0} сделок.");
                }
                else
                {
                    DealData = new DealData();
                    SaveDeals();
                    System.Diagnostics.Debug.WriteLine($"DataStorage LoadDeals: Файл {fullPath} не найден, создан новый пустой DealData.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка при загрузке сделок: {ex.Message}");
                DealData = new DealData();
            }
        }

        public static void SaveDeals()
        {
            try
            {
                string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "deals.json");
                string json = JsonConvert.SerializeObject(DealData, Formatting.Indented);
                File.WriteAllText(fullPath, json);
                System.Diagnostics.Debug.WriteLine($"DataStorage SaveDeals: Файл {fullPath} сохранён. Содержимое:\n{json}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка при сохранении сделок: {ex.Message}");
            }
        }
    }
}
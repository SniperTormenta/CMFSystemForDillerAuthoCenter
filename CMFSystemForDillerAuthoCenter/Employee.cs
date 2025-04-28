using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMFSystemForDillerAuthoCenter
{
    public class Employee
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Position { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime CreatedDate { get; set; } // Добавляем
        public DateTime ModifiedDate { get; set; } // Добавляем
    }

    public class EmployeeStorage
    {
        public List<Employee> Employees { get; set; } = new List<Employee>();

        private static readonly string _storagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "employees.json");

        public void Save()
        {
            try
            {
                string json = JsonConvert.SerializeObject(this, Formatting.Indented);
                File.WriteAllText(_storagePath, json);
                System.Diagnostics.Debug.WriteLine($"EmployeeStorage Save: Файл {_storagePath} сохранён. Содержимое:\n{json}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка при сохранении сотрудников: {ex.Message}");
            }
        }

        public static EmployeeStorage Load()
        {
            try
            {
                if (File.Exists(_storagePath))
                {
                    string json = File.ReadAllText(_storagePath);
                    var storage = JsonConvert.DeserializeObject<EmployeeStorage>(json) ?? new EmployeeStorage();
                    System.Diagnostics.Debug.WriteLine($"EmployeeStorage Load: Загружено {storage.Employees.Count} сотрудников.");
                    return storage;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка при загрузке сотрудников: {ex.Message}");
            }
            return new EmployeeStorage();
        }
    }
}
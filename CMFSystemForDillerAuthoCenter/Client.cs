using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMFSystemForDillerAuthoCenter
{
    public class ContactPerson
    {
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Position { get; set; }
        public string Notes { get; set; }
    }
    public class Client
    {
        public string Id { get; set; }
        public string Type { get; set; } // Физлицо или Юрлицо
        public string FirstName { get; set; } // Для Физлица
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Gender { get; set; }
        public string CompanyName { get; set; } // Для Юрлица
        public string Email { get; set; }
        public string Phone { get; set; }
        public List<ContactPerson> ContactPersons { get; set; } = new List<ContactPerson>(); // Только для Юрлица
        public string Category { get; set; } // Клиент, Конкурент, Партнёр
        public string Tag { get; set; } // Метка для фильтрации
        public string Notes { get; set; }
        public string Responsible { get; set; } // Ответственный сотрудник
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        [JsonIgnore]
        public string ClientName
        {
            get
            {
                if (Type == "Юрлицо")
                    return CompanyName;
                return $"{LastName} {FirstName} {MiddleName}".Trim();
            }
        }
    }

    public class ClientFilter
    {
        public string Name { get; set; }
        public string SortBy { get; set; }
        public DateTime? DateFilter { get; set; }
        public string TypeFilter { get; set; }
        public string CategoryFilter { get; set; }
        public string ResponsibleFilter { get; set; }
        public string TagFilter { get; set; }
    }

    public class ClientStorage
    {
        public List<Client> Clients { get; set; } = new List<Client>();
        public List<ClientFilter> SavedFilters { get; set; } = new List<ClientFilter>();

        private static readonly string _storagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "clients.json");

        public void Save()
        {
            try
            {
                string json = JsonConvert.SerializeObject(this, Formatting.Indented);
                File.WriteAllText(_storagePath, json);
                System.Diagnostics.Debug.WriteLine($"ClientStorage Save: Файл {_storagePath} сохранён. Содержимое:\n{json}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка при сохранении клиентов: {ex.Message}");
            }
        }

        public static ClientStorage Load()
        {
            try
            {
                if (File.Exists(_storagePath))
                {
                    string json = File.ReadAllText(_storagePath);
                    var storage = JsonConvert.DeserializeObject<ClientStorage>(json) ?? new ClientStorage();
                    System.Diagnostics.Debug.WriteLine($"ClientStorage Load: Загружено {storage.Clients.Count} клиентов.");
                    return storage;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка при загрузке клиентов: {ex.Message}");
            }
            return new ClientStorage();
        }
    }
}
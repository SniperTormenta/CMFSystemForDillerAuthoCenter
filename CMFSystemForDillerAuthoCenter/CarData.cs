using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace CMFSystemForDillerAuthoCenter
{
    public class CarData
    {
        public List<Car> Cars { get; set; } = new List<Car>();
    }

    public class Car
    {
        public string Id { get; set; }
        public string SiteCode { get; set; }
        public string StsSeries { get; set; }
        public string StsNumber { get; set; }
        public string OwnerLastName { get; set; }
        public string OwnerFirstName { get; set; }
        public string OwnerMiddleName { get; set; }
        public string OwnerAddress { get; set; }
        public string LicensePlate { get; set; }
        public string LicensePlateRegion { get; set; }
        public string Vin { get; set; }
        public string BodyNumber { get; set; }
        public string VehicleCategory { get; set; }
        public int Year { get; set; }
        public string Color { get; set; }
        public string EnginePower { get; set; }
        public string EnvironmentalClass { get; set; }
        public string MaxPermittedWeight { get; set; }
        public string UnladenWeight { get; set; }
        public string PtsSeries { get; set; }
        public string PtsNumber { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string PhotoPath { get; set; }
        public string DisplayName { get; set; } // Добавляем свойство для отображения в ComboBox
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
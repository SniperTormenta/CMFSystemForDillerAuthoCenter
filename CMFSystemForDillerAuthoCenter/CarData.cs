using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMFSystemForDillerAuthoCenter
{
    public class CarData
    {
        public List<Car> Cars { get; set; }

        public CarData()
        {
            Cars = new List<Car>();

        }
    }


    public class Car
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string Engine { get; set; }
        public string Drive { get; set; }
        public string BodyType { get; set; }
        public double EngineVolume { get; set; }
        public int Mileage { get; set; }
        public string Color { get; set; }
        public string SteeringWheel { get; set; }
        public string Interior { get; set; }
        public string OwnershipDuration { get; set; }
        public int OwnerCount { get; set; }
        public string Condition { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
        public string LastOwner { get; set; }
        public string OwnerCode { get; set; }
        public string LocationCode { get; set; }
        public string PhotoPath { get; set; }
    }
}
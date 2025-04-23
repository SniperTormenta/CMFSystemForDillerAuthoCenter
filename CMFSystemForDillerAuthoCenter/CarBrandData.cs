using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMFSystemForDillerAuthoCenter
{
    public class CarBrandData
    {
        public List<CarBrand> Brands { get; set; }

        public CarBrandData()
        {
            Brands = new List<CarBrand>();
        }
    }

    public class CarBrand
    {
        public string Name { get; set; }
        public string LogoPath { get; set; }
    }
}
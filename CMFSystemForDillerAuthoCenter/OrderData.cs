using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMFSystemForDillerAuthoCenter
{
    public class OrderData
    {
        public List<Order> Orders { get; set; }

        public OrderData()
        {
            Orders = new List<Order>();
        }
    }

    public class Order
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public DateTime Date { get; set; }
        public string CarModel { get; set; }
        public decimal Price { get; set; }
        public string ClientName { get; set; }
        public string ClientPhone { get; set; }
        public string Source { get; set; }
    }
}
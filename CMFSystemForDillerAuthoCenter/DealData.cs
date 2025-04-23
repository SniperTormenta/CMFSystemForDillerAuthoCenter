using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMFSystemForDillerAuthoCenter
{
    public class DealData
    {
        public List<Deal> Deals { get; set; }

        public DealData()
        {
            Deals = new List<Deal>();
        }
    }

    public class Deal
    {
        public string Id { get; set; }
        public string Type { get; set; } // "Appeal" или "Order"
        public string ClientName { get; set; }
        public string ClientPhone { get; set; }
        public string ClientEmail { get; set; }
        public string Date { get; set; }
        public string Theme { get; set; } // Для обращений
        public string Notes { get; set; } // Подробности
        public string Status { get; set; }
        public string CarId { get; set; } // Для заказов
        public decimal Amount { get; set; } // Для заказов
        public string PaymentTerms { get; set; } // Для заказов
        public bool IsDeliveryRequired { get; set; } // Для заказов: нужна ли доставка
        public string DeliveryDate { get; set; } // Для заказов
        public string DeliveryAddress { get; set; } // Для заказов
    }
}
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
    public string Type { get; set; } // "Обращение" или "Заказ"
    public string ClientId { get; set; }
    public string ClientName { get; set; }
    public string ClientPhone { get; set; }
    public string ClientEmail { get; set; }
    public string Date { get; set; }
    public string Theme { get; set; } // Для обращений
    public string Notes { get; set; } // Подробности
    public string Status { get; set; } // Оставляем для совместимости
    public string AppealStatus { get; set; } // Для "Обращения"
    public string PaymentStatus { get; set; } // Для "Заказа"
    public string OrderStatus { get; set; } // Для "Заказа"
    public string CarId { get; set; }
    public string CarInfo { get; internal set; }
    public decimal Amount { get; set; } // Для заказов
    public string PaymentTerms { get; set; } // Для заказов
    public bool IsDeliveryRequired { get; set; } // Для заказов: нужна ли доставка
    public string DeliveryDate { get; set; } // Для заказов
    public string DeliveryAddress { get; set; } // Для заказов
    public string ServicedBy { get; set; } // ID сотрудника, который обслуживал заказ/обращение
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
}
}
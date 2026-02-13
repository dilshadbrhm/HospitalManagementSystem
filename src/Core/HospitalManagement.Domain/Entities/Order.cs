using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Domain.Entities
{
    public class Order : BaseEntity
    {
        public string UserId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public string ShippingAddress { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string PaymentMethod { get; set; } 
        public string PaymentStatus { get; set; }
        public DateTime OrderDate { get; set; } 
        public string Notes { get; set; }

        public ICollection<OrderItem> Items { get; set; }
    }
}

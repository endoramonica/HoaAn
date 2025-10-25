using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VietCommerce.Core.Common;
using VietCommerce.Core.Entities.Users;
using VietCommerce.Core.Enums.Orders;
namespace VietCommerce.Core.Entities.Orders
{
    public class OrderStatusHistory : AuditableEntity
    {
        public Guid OrderId { get; set; }
        public OrderStatus Status { get; set; } 
        public string? Notes { get; set; }
        // Navigation properties
        public virtual Order Order { get; set; } = null!;
        public virtual User? CreatedByUser { get; set; }
    }
}

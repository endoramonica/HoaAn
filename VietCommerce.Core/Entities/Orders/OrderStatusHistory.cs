using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
        public OrderStatus OldStatus { get; set; }
        public OrderStatus NewStatus { get; set; }
        public Guid ChangedBy { get; set; }
        public string? Notes { get; set; }
        public DateTime ChangedAt { get; set; }
        // Navigation properties
        public virtual Order Order { get; set; } = null!;
        public virtual User? CreatedByUser { get; set; }
        [ForeignKey("ChangedBy")]
        public virtual User? ChangedByUser { get; set; }
        


    }
}

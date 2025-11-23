using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VietCommerce.Core.DTOs.Customers;

namespace VietCommerce.Core.DTOs.Comments
{
    public class CommentPreviewDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime AddedOn { get; set; }
        public string? CustomerName { get; set; }
    }
    public class CommentDto
    {
        public Guid Id { get; set; }
        public Guid PostId { get; set; }
        public Guid CustomerId { get; set; }
        public string Content { get; set; }
        public DateTime AddedOn { get; set; }
        public CustomerDto Customer { get; set; }
    }

}

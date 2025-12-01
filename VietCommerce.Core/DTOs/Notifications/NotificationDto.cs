using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VietCommerce.Core.Enums.Notifications;

namespace VietCommerce.Core.DTOs.Notifications
{
    public class NotificationDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid? TemplateId { get; set; }
        public string? Title { get; set; }
        public string? Message { get; set; }
        public NotificationType? Type { get; set; }
        public bool Read { get; set; }
        public DateTime CreatedAt { get; set; }

        // Extra info từ Template (rất hay dùng ở FE)
        public string? TemplateName { get; set; }
        
    }
}

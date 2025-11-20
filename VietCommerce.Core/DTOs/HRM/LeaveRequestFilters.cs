using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VietCommerce.Core.Entities.HRM;

namespace VietCommerce.Core.DTOs.HRM
{
    /// <summary>
    /// Filter criteria for leave requests
    /// </summary>
    public class LeaveRequestFilters
    {
        /// <summary>
        /// Filter by employee ID
        /// </summary>
        public Guid? EmployeeId { get; set; }

        /// <summary>
        /// Filter by leave request status
        /// </summary>
        public LeaveRequestStatus? Status { get; set; }

        /// <summary>
        /// Filter by leave type
        /// </summary>
        public LeaveRequestType? LeaveType { get; set; }

        /// <summary>
        /// Filter by start date (from)
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Filter by end date (to)
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Filter by submitted date (from)
        /// </summary>
        public DateTime? SubmittedFrom { get; set; }

        /// <summary>
        /// Filter by submitted date (to)
        /// </summary>
        public DateTime? SubmittedTo { get; set; }

        /// <summary>
        /// General search term (searches in Reason, Comments, EmployeeName)
        /// </summary>
        public string? SearchTerm { get; set; }
    }
}

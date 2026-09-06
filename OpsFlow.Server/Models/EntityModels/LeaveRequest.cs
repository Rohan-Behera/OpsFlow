using OpsFlow.Server.Enum;
using System.ComponentModel.DataAnnotations;

namespace OpsFlow.Server.Models.EntityModels
{
    public class LeaveRequest
    {
        public int LeaveRequestId { get; set; }
        public int EmployeeId { get; set; }
        public int LeaveTypeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalDays { get; set; }
        public string? Reason { get; set; }
        public LeaveRequestStatusEnum Status { get; set; } = LeaveRequestStatusEnum.Pending;
        public int? ApprovedByEmployeeId { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string? RejectionReason { get; set; }
        public DateTime RequestedAt { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; } = null!;

        // Navigation
        public Employee Employee { get; set; } = null!;
        public LeaveType LeaveType { get; set; } = null!;
        public Employee? ApprovedByEmployee { get; set; }
    }
}

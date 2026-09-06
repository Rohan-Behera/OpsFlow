using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpsFlow.Server.Models.EntityModels
{
    public class LeaveBalance
    {
        public int LeaveBalanceId { get; set; }
        public int EmployeeId { get; set; }
        public int LeaveTypeId { get; set; }
        public short Year { get; set; }
        public decimal AllocatedDays { get; set; }
        public decimal CarriedOverDays { get; set; }
        public decimal UsedDays { get; set; }

        // Computed column (AllocatedDays + CarriedOverDays - UsedDays) PERSISTED — DB-generated, read-only
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal RemainingDays { get; set; }

        public DateTime LastUpdatedAt { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; } = null!;

        // Navigation
        public Employee Employee { get; set; } = null!;
        public LeaveType LeaveType { get; set; } = null!;
    }
}

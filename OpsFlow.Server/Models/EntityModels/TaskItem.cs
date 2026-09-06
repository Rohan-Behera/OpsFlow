using OpsFlow.Server.Enum;
using System.ComponentModel.DataAnnotations;

namespace OpsFlow.Server.Models.EntityModels
{
    public class TaskItem
    {
        public int TaskId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public int? AssignedToEmployeeId { get; set; }
        public int? AssignedByEmployeeId { get; set; }
        public int? DepartmentId { get; set; }
        public TaskStatusEnum Status { get; set; } = TaskStatusEnum.Open;
        public TaskPriorityEnum Priority { get; set; } = TaskPriorityEnum.Low;
        public DateTime? DueDate { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime CreatedAt { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; } = null!;

        // Navigation
        public Employee? AssignedToEmployee { get; set; }
        public Employee? AssignedByEmployee { get; set; }
        public Department? Department { get; set; }
    }
}

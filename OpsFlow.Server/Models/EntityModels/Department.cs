namespace OpsFlow.Server.Models.EntityModels
{
    public class Department
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = null!;
        public int? ManagerEmployeeId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation
        public Employee? ManagerEmployee { get; set; }
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }
}

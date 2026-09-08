using Microsoft.EntityFrameworkCore;
using OpsFlow.Server.Models.DTOModels.AuthDTO;
using OpsFlow.Server.Models.EntityModels;

namespace OpsFlow.Server.Core
{
    public class OpsFlowContext : DbContext
    {
        public OpsFlowContext(DbContextOptions<OpsFlowContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();
        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<Department> Departments => Set<Department>();
        public DbSet<TaskItem> TaskItems => Set<TaskItem>();
        public DbSet<LeaveBalance> LeaveBalances => Set<LeaveBalance>();
        public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();
        public DbSet<LeaveType> LeaveTypes => Set<LeaveType>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        public DbSet<RefreshTokens> RefreshTokens => Set<RefreshTokens>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ----------Users-------------
            modelBuilder.Entity<User>(e =>
            {
                e.ToTable("Users");
                e.HasKey(x => x.UserId);
                e.Property(x => x.UserName).IsRequired().HasMaxLength(100);
                e.Property(x => x.Email).HasMaxLength(255);
                e.HasIndex(x => x.UserName).IsUnique();
                e.HasIndex(x => x.Email).IsUnique();
                e.Property(x => x.RowVersion).IsRowVersion();

                // One User -> one Employee (optional)
                e.HasOne(x => x.Employee)
                 .WithOne(x => x.User)
                 .HasForeignKey<Employee>(x => x.UserId);
            });

            //----------Roles-------------
            modelBuilder.Entity<Role>(e =>
            {
                e.ToTable("Roles");
                e.HasKey(x => x.RoleId);
                e.Property(x => x.RoleName).HasMaxLength(50).IsRequired();
                e.HasIndex(x => x.RoleName).IsUnique();
            });

            //----------UserRoles (many-to-many with payload)-------------
            modelBuilder.Entity<UserRole>(e =>
            {
                e.ToTable("UserRoles");
                e.HasKey(x => x.UserRoleId);
                e.HasIndex(x => new { x.UserId, x.RoleId }).IsUnique();

                e.HasOne(x => x.User)
                 .WithMany(x => x.UserRoles)
                 .HasForeignKey(x => x.UserId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.Role)
                 .WithMany(x => x.UserRoles)
                 .HasForeignKey(x => x.RoleId)
                 .OnDelete(DeleteBehavior.Cascade);

                // Optional "granted by" user — must NOT cascade, or SQL Server
                // will reject it as a second cascade path into Users.
                e.HasOne(x => x.AssignedByUser)
                 .WithMany()
                 .HasForeignKey(x => x.AssignedByUserId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            //----------Employees-------------
            modelBuilder.Entity<Employee>(e =>
            {
                e.ToTable("Employees");
                e.HasKey(x => x.EmployeeId);
                e.Property(x => x.EmployeeCode).HasMaxLength(20).IsRequired();
                e.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
                e.Property(x => x.LastName).HasMaxLength(100).IsRequired();
                e.Property(x => x.RowVersion).IsRowVersion();
                e.HasIndex(x => x.EmployeeCode).IsUnique();

                e.HasOne(x => x.Department)
                 .WithMany(x => x.Employees)
                 .HasForeignKey(x => x.DepartmentId)
                 .OnDelete(DeleteBehavior.Restrict);

                // Self-referencing Manager relationship — must be Restrict,
                // SQL Server won't allow cascade delete on a self-reference.
                e.HasOne(x => x.Manager)
                 .WithMany(x => x.DirectReports)
                 .HasForeignKey(x => x.ManagerId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            //----------Leave Types-------------
            modelBuilder.Entity<LeaveType>(e =>
            {
                e.ToTable("LeaveTypes");
                e.HasKey(x => x.LeaveTypeId);
                e.Property(x => x.LeaveTypeName).HasMaxLength(50).IsRequired();
                e.Property(x => x.DefaultAnnualDays).HasColumnType("decimal(5,2)");
                e.HasIndex(x => x.LeaveTypeName).IsUnique();
            });

            //----------Leave Balances-------------
            modelBuilder.Entity<LeaveBalance>(e =>
            {
                e.ToTable("LeaveBalances");
                e.HasKey(x => x.LeaveBalanceId);
                e.Property(x => x.AllocatedDays).HasColumnType("decimal(5,2)");
                e.Property(x => x.CarriedOverDays).HasColumnType("decimal(5,2)");
                e.Property(x => x.UsedDays).HasColumnType("decimal(5,2)");
                e.Property(x => x.RowVersion).IsRowVersion();

                // Matches: RemainingDays AS (AllocatedDays + CarriedOverDays - UsedDays) PERSISTED
                e.Property(x => x.RemainingDays)
                 .HasColumnType("decimal(5,2)")
                 .HasComputedColumnSql("([AllocatedDays]+[CarriedOverDays]-[UsedDays])", stored: true);

                e.HasIndex(x => new { x.EmployeeId, x.LeaveTypeId, x.Year }).IsUnique();

                e.HasOne(x => x.Employee)
                 .WithMany(x => x.LeaveBalances)
                 .HasForeignKey(x => x.EmployeeId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.LeaveType)
                 .WithMany(x => x.LeaveBalances)
                 .HasForeignKey(x => x.LeaveTypeId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            //----------Leave Requests-------------
            modelBuilder.Entity<LeaveRequest>(e =>
            {
                e.ToTable("LeaveRequests");
                e.HasKey(x => x.LeaveRequestId);
                e.Property(x => x.TotalDays).HasColumnType("decimal(5,2)");
                e.Property(x => x.Status)
                 .HasConversion<string>()
                 .HasMaxLength(20)
                 .IsRequired();
                e.Property(x => x.RowVersion).IsRowVersion();

                e.HasOne(x => x.Employee)
                 .WithMany(x => x.LeaveRequests)
                 .HasForeignKey(x => x.EmployeeId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.LeaveType)
                 .WithMany(x => x.LeaveRequests)
                 .HasForeignKey(x => x.LeaveTypeId)
                 .OnDelete(DeleteBehavior.Restrict);

                // Second FK into Employees — Restrict to avoid multiple cascade paths
                e.HasOne(x => x.ApprovedByEmployee)
                 .WithMany(x => x.ApprovedLeaveRequests)
                 .HasForeignKey(x => x.ApprovedByEmployeeId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            //----------Task Items-------------
            modelBuilder.Entity<TaskItem>(e =>
            {
                e.ToTable("Tasks");
                e.HasKey(x => x.TaskId);
                e.Property(x => x.Title).HasMaxLength(200).IsRequired();
                e.Property(x => x.Status)
                 .HasConversion<string>()
                 .HasMaxLength(20)
                 .IsRequired();
                e.Property(x => x.Priority)
                 .HasConversion<string>()
                 .HasMaxLength(10)
                 .IsRequired();
                e.Property(x => x.RowVersion).IsRowVersion();

                e.HasOne(x => x.AssignedToEmployee)
                 .WithMany(x => x.AssignedTasks)
                 .HasForeignKey(x => x.AssignedToEmployeeId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.AssignedByEmployee)
                 .WithMany(x => x.CreatedTasks)
                 .HasForeignKey(x => x.AssignedByEmployeeId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Department)
                 .WithMany(x => x.Tasks)
                 .HasForeignKey(x => x.DepartmentId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            //----------Audit Logs-------------
            modelBuilder.Entity<AuditLog>(e =>
            {
                e.ToTable("AuditLogs");
                e.HasKey(x => x.AuditLogId);
                e.Property(x => x.Action).HasMaxLength(100).IsRequired();
                e.Property(x => x.TableName).HasMaxLength(128);
                e.Property(x => x.RecordId).HasMaxLength(50);
                e.Property(x => x.IPAddress).HasMaxLength(45);

                e.HasOne(x => x.User)
                 .WithMany(x => x.AuditLogs)
                 .HasForeignKey(x => x.UserId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            //----------Refresh Tokens-------------
            modelBuilder.Entity<RefreshTokens>(e =>
            {
                e.ToTable("RefreshTokens");
                e.HasKey(x => x.Id);
                e.HasIndex(x => x.UserId);
                e.HasOne(x => x.User)
                 .WithMany(x => x.RefreshTokens)
                 .HasForeignKey(x => x.UserId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

        }
    }
}

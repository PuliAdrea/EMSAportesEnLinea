using EMS.Domain.Enums;

namespace EMS.API.DTOs.EmployeeDTOs
{
    public class EmployeeCreateDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public decimal BaseSalary { get; set; }
        public JobPosition Position { get; set; }
        public int DepartmentId { get; set; }
    }
}

namespace EMS.API.DTOs.EmployeeDTOs
{
    public class EmployeeResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public decimal CalculatedSalary { get; set; }
        public string Position { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
    }
}

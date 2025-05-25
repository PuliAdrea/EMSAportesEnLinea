namespace EMS.API.DTOs.DepartmentDTOs
{
    public class DepartmentResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal TotalSalaryExpenditure { get; set; }
    }
}

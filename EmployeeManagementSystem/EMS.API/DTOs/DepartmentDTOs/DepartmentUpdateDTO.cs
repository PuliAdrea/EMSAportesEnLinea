using System.ComponentModel.DataAnnotations;

namespace EMS.API.DTOs.DepartmentDTOs
{
    public class DepartmentUpdateDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}

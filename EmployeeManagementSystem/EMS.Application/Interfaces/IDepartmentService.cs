using EMS.Domain.Entities;

namespace EMS.Application.Interfaces;

public interface IDepartmentService
{
    Task<Department> CreateDepartmentAsync(Department department);
    Task<IEnumerable<Department>> GetAllDepartmentsAsync();
    Task<Department?> GetDepartmentByIdAsync(int id);
    Task UpdateDepartmentAsync(Department department);
    Task DeleteDepartmentAsync(int id);
}
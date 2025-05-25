using EMS.Domain.Entities;

namespace EMS.Application.Interfaces;

public interface IEmployeeService
{
    Task<IEnumerable<Employee>> GetAllEmployeesAsync();
    Task<Employee?> GetEmployeeByIdAsync(int id);
    Task<decimal> CalculateSalaryAsync(int employeeId);
    Task<decimal> CalculateDepartmentSalaryAsync(int departmentId);
    Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(int departmentId);
    Task UpdateEmployeeAsync(Employee employee);
    Task DeleteEmployeeAsync(int id);
}
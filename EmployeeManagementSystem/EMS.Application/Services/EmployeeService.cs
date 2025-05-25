using EMS.Application.Interfaces;
using EMS.Domain.Entities;
using EMS.Domain.Enums;
using EMS.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;

namespace EMS.Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<EmployeeService> _logger;

    public EmployeeService(IUnitOfWork unitOfWork, ILogger<EmployeeService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
    {
        return await _unitOfWork.Employees.GetAllAsync();
    }

    public async Task<Employee?> GetEmployeeByIdAsync(int id)
    {
        return await _unitOfWork.Employees.GetByIdAsync(id);
    }
    public async Task<decimal> CalculateSalaryAsync(int employeeId)
    {
        var employee = await _unitOfWork.Employees.GetByIdAsync(employeeId);
        if (employee == null) throw new KeyNotFoundException("Employee not found");

        return employee.Position switch
        {
            JobPosition.Developer => employee.BaseSalary * 1.10m,
            JobPosition.Manager => employee.BaseSalary * 1.20m,
            JobPosition.HR or JobPosition.Sales => employee.BaseSalary,
            _ => throw new InvalidOperationException("Invalid job position")
        };
    }

    public async Task<decimal> CalculateDepartmentSalaryAsync(int departmentId)
    {
        var employees = (await _unitOfWork.Employees.FindAsync(e => e.DepartmentId == departmentId)).ToList();
        decimal total = 0;

        foreach (var employee in employees)
        {
            total += await CalculateSalaryAsync(employee.Id);
        }

        return total;
    }

    public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(int departmentId)
        => await _unitOfWork.Employees.FindAsync(e => e.DepartmentId == departmentId);

    public async Task UpdateEmployeeAsync(Employee employee)
    {
        var existingEmployee = await _unitOfWork.Employees.GetByIdAsync(employee.Id);

        if (existingEmployee == null)
            throw new KeyNotFoundException("Employee not found");

        existingEmployee.Name = employee.Name;
        existingEmployee.Email = employee.Email;
        existingEmployee.BaseSalary = employee.BaseSalary;
        existingEmployee.Position = employee.Position;
        existingEmployee.DepartmentId = employee.DepartmentId;

        _unitOfWork.Employees.Update(existingEmployee);
        await _unitOfWork.CompleteAsync();
    }

    public async Task DeleteEmployeeAsync(int id)
    {
        var employee = await _unitOfWork.Employees.GetByIdAsync(id);

        if (employee == null)
            throw new KeyNotFoundException("Employee not found");

        _unitOfWork.Employees.Remove(employee);
        await _unitOfWork.CompleteAsync();
    }

}
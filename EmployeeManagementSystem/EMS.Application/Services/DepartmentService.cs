using EMS.Application.Interfaces;
using EMS.Domain.Entities;
using EMS.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;

namespace EMS.Application.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DepartmentService> _logger;

    public DepartmentService(IUnitOfWork unitOfWork, ILogger<DepartmentService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<IEnumerable<Department>> GetAllDepartmentsAsync()
    {
        return await _unitOfWork.Departments.GetAllAsync();
    }

    public async Task<Department?> GetDepartmentByIdAsync(int id)
    {
        return await _unitOfWork.Departments.GetByIdAsync(id);
    }

    public async Task<Department> CreateDepartmentAsync(Department department)
    {
        try
        {
            await _unitOfWork.Departments.AddAsync(department);
            await _unitOfWork.CompleteAsync();
            return department;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating department");
            throw;
        }
    }


    public async Task UpdateDepartmentAsync(Department department)
    {
        var existingDepartment = await _unitOfWork.Departments.GetByIdAsync(department.Id);

        if (existingDepartment == null)
            throw new KeyNotFoundException("Department not found");

        existingDepartment.Name = department.Name;

        _unitOfWork.Departments.Update(existingDepartment);
        await _unitOfWork.CompleteAsync();
    }

    public async Task DeleteDepartmentAsync(int id)
    {
        var department = await _unitOfWork.Departments.GetByIdAsync(id);

        if (department == null)
            throw new KeyNotFoundException("Department not found");

        var hasEmployees = (await _unitOfWork.Employees.FindAsync(e => e.DepartmentId == id)).Any();

        if (hasEmployees)
            throw new InvalidOperationException("You cannot delete a department with employees");

        _unitOfWork.Departments.Remove(department);
        await _unitOfWork.CompleteAsync();
    }

}
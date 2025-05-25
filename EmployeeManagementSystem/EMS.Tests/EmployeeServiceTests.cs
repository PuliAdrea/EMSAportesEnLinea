using EMS.Application.Services;
using EMS.Domain.Entities;
using EMS.Domain.Enums;
using EMS.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace EMS.Tests;

public class EmployeeServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ILogger<EmployeeService>> _mockLogger;
    private readonly EmployeeService _employeeService;

    public EmployeeServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockLogger = new Mock<ILogger<EmployeeService>>();
        _employeeService = new EmployeeService(_mockUnitOfWork.Object, _mockLogger.Object);
    }

    //ejemplo prueba tecnica
    [Fact]
    public async Task GetEmployeeById_ShouldReturnEmployee_WhenEmployeeExists()
    {
        // Arrange
        var employee = new Employee
        {
            Id = 1,
            Name = "John Doe",
            Email = "john@example.com",
            BaseSalary = 5000,
            Position = JobPosition.Developer,
            DepartmentId = 1
        };

        _mockUnitOfWork.Setup(u => u.Employees.GetByIdAsync(1))
                      .ReturnsAsync(employee);

        // Act
        var result = await _employeeService.GetEmployeeByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("John Doe", result.Name);
        Assert.Equal("john@example.com", result.Email);
        Assert.Equal(JobPosition.Developer, result.Position);
    }

    [Fact]
    public async Task GetEmployeeById_ShouldReturnNull_WhenEmployeeNotExists()
    {
        // Arrange
        _mockUnitOfWork.Setup(u => u.Employees.GetByIdAsync(It.IsAny<int>()))
                      .ReturnsAsync((Employee)null);

        // Act
        var result = await _employeeService.GetEmployeeByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

   
    [Fact]
    public async Task CalculateSalary_ShouldReturnCorrectSalary_ForDeveloper()
    {
        // Arrange
        var developer = new Employee
        {
            Id = 1,
            BaseSalary = 5000,
            Position = JobPosition.Developer
        };

        _mockUnitOfWork.Setup(u => u.Employees.GetByIdAsync(1))
                      .ReturnsAsync(developer);

        // Act
        var result = await _employeeService.CalculateSalaryAsync(1);

        // Assert (Developer: +10%)
        Assert.Equal(5500, result);
    }

    [Fact]
    public async Task CalculateSalary_ShouldThrow_WhenEmployeeNotFound()
    {
        // Arrange
        _mockUnitOfWork.Setup(u => u.Employees.GetByIdAsync(It.IsAny<int>()))
                      .ReturnsAsync((Employee)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _employeeService.CalculateSalaryAsync(999));
    }

    [Fact]
    public async Task DeleteEmployee_ShouldComplete_WhenEmployeeExists()
    {
        // Arrange
        var employee = new Employee { Id = 1 };
        _mockUnitOfWork.Setup(u => u.Employees.GetByIdAsync(1))
                      .ReturnsAsync(employee);

        // Act
        await _employeeService.DeleteEmployeeAsync(1);

        // Assert
        _mockUnitOfWork.Verify(u => u.Employees.Remove(employee), Times.Once);
        _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteEmployee_ShouldThrow_WhenEmployeeNotExists()
    {
        // Arrange
        _mockUnitOfWork.Setup(u => u.Employees.GetByIdAsync(It.IsAny<int>()))
                      .ReturnsAsync((Employee)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _employeeService.DeleteEmployeeAsync(999));
    }

    [Fact]
    public async Task UpdateEmployee_ShouldUpdateProperties_WhenEmployeeExists()
    {
        // Arrange
        var existingEmployee = new Employee
        {
            Id = 1,
            Name = "Old Name",
            Email = "old@example.com",
            BaseSalary = 4000,
            Position = JobPosition.HR,
            DepartmentId = 1
        };

        var updatedEmployee = new Employee
        {
            Id = 1,
            Name = "New Name",
            Email = "new@example.com",
            BaseSalary = 4500,
            Position = JobPosition.Manager,
            DepartmentId = 2
        };

        _mockUnitOfWork.Setup(u => u.Employees.GetByIdAsync(1))
                      .ReturnsAsync(existingEmployee);

        // Act
        await _employeeService.UpdateEmployeeAsync(updatedEmployee);

        // Assert
        Assert.Equal("New Name", existingEmployee.Name);
        Assert.Equal("new@example.com", existingEmployee.Email);
        Assert.Equal(4500, existingEmployee.BaseSalary);
        Assert.Equal(JobPosition.Manager, existingEmployee.Position);
        Assert.Equal(2, existingEmployee.DepartmentId);
        _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllEmployees_ShouldReturnAllEmployees()
    {
        // Arrange
        var employees = new List<Employee>
        {
            new() { Id = 1, Name = "Employee 1" },
            new() { Id = 2, Name = "Employee 2" }
        };

        _mockUnitOfWork.Setup(u => u.Employees.GetAllAsync())
                      .ReturnsAsync(employees);

        // Act
        var result = await _employeeService.GetAllEmployeesAsync();

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, e => e.Name == "Employee 1");
        Assert.Contains(result, e => e.Name == "Employee 2");
    }
}
using EMS.Application.Services;
using EMS.Domain.Entities;
using EMS.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;

namespace EMS.Tests;

public class DepartmentServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ILogger<DepartmentService>> _mockLogger;
    private readonly DepartmentService _departmentService;

    public DepartmentServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockLogger = new Mock<ILogger<DepartmentService>>();
        _departmentService = new DepartmentService(_mockUnitOfWork.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetDepartmentById_ShouldReturnDepartment_WhenExists()
    {
        // Arrange
        var department = new Department { Id = 1, Name = "IT" };
        _mockUnitOfWork.Setup(u => u.Departments.GetByIdAsync(1))
                      .ReturnsAsync(department);

        // Act
        var result = await _departmentService.GetDepartmentByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("IT", result.Name);
    }

    [Fact]
    public async Task CreateDepartment_ShouldReturnCreatedDepartment()
    {
        // Arrange
        var newDepartment = new Department { Name = "HR" };
        _mockUnitOfWork.Setup(u => u.Departments.AddAsync(It.IsAny<Department>()))
                      .Returns(Task.CompletedTask)
                      .Callback<Department>(d => d.Id = 2);
        _mockUnitOfWork.Setup(u => u.CompleteAsync()).ReturnsAsync(1);

        // Act
        var result = await _departmentService.CreateDepartmentAsync(newDepartment);

        // Assert
        Assert.Equal(2, result.Id);
        Assert.Equal("HR", result.Name);
    }

    [Fact]
    public async Task DeleteDepartment_ShouldThrow_WhenDepartmentHasEmployees()
    {
        // Arrange
        var department = new Department { Id = 1 };
        _mockUnitOfWork.Setup(u => u.Departments.GetByIdAsync(1))
                      .ReturnsAsync(department);
        _mockUnitOfWork.Setup(u => u.Employees.FindAsync(It.IsAny<Expression<Func<Employee, bool>>>()))
                      .ReturnsAsync(new List<Employee> { new Employee() });

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _departmentService.DeleteDepartmentAsync(1));
    }

}
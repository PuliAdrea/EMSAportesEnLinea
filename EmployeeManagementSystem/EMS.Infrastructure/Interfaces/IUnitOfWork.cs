using EMS.Domain.Entities;

namespace EMS.Infrastructure.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Employee> Employees { get; }
        IRepository<Department> Departments { get; }
        Task<int> CompleteAsync();
    }
}

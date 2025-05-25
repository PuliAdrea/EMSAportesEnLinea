using EMS.Domain.Entities;
using EMS.Infrastructure.Data;
using EMS.Infrastructure.Interfaces;

namespace EMS.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private Repository<Employee>? _employees;
    private Repository<Department>? _departments;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IRepository<Employee> Employees =>
        _employees ??= new Repository<Employee>(_context);

    public IRepository<Department> Departments =>
        _departments ??= new Repository<Department>(_context);

    public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
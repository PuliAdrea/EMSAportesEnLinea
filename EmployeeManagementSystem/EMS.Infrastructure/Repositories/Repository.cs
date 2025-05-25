using EMS.Infrastructure.Data;
using EMS.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EMS.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _entities;

    public Repository(AppDbContext context)
    {
        _context = context;
        _entities = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(int id) => await _entities.FindAsync(id);
    public async Task<IEnumerable<T>> GetAllAsync() => await _entities.ToListAsync();
    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        => await _entities.Where(predicate).ToListAsync();

    public async Task AddAsync(T entity) => await _entities.AddAsync(entity);
    public async Task AddRangeAsync(IEnumerable<T> entities) => await _entities.AddRangeAsync(entities);
    public void Update(T entity) => _entities.Update(entity);
    public void Remove(T entity) => _entities.Remove(entity);
    public void RemoveRange(IEnumerable<T> entities) => _entities.RemoveRange(entities);
}
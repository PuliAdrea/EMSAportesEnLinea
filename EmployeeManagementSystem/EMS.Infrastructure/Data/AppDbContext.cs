using EMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EMS.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        SavingChanges += (sender, args) =>
        {
            foreach (var entry in ChangeTracker.Entries()
                .Where(e => e.State is EntityState.Added or EntityState.Modified))
            {
            }
        };
    }

    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Department> Departments => Set<Department>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>()
            .Property(e => e.BaseSalary)
            .HasPrecision(18, 2);
    }
}
using System.Reflection;
using BackgroundService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BackgroundService.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Currency> Currencies { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {        
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }
}
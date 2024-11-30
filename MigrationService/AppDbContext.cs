using System.Reflection;
using Microsoft.EntityFrameworkCore;
using MigrationService.Models;

namespace MigrationService;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Currency> Currencies { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserCurrency> UserCurrencies { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {        
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }
}
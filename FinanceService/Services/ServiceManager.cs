using FinanceService.Infrastructure.Persistent;

namespace FinanceService.Services;

public class ServiceManager(FinanceDbContext context)
{
    public FinanceDbContext Context { get; } = context;
}
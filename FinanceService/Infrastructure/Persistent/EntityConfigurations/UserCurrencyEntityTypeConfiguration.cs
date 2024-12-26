using FinanceService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceService.Infrastructure.Persistent.EntityConfigurations;

public class UserCurrencyEntityTypeConfiguration : IEntityTypeConfiguration<UserCurrency>
{
    public void Configure(EntityTypeBuilder<UserCurrency> builder)
    {
        builder.ToTable(nameof(UserCurrency));
        builder.HasKey(x => x.Id);
    }
}
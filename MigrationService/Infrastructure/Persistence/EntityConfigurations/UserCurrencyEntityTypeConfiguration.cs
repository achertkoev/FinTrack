using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MigrationService.Domain.Entities;

namespace MigrationService.Infrastructure.Persistence.EntityConfigurations;

public class UserCurrencyEntityTypeConfiguration : IEntityTypeConfiguration<UserCurrency>
{
    public void Configure(EntityTypeBuilder<UserCurrency> builder)
    {
        builder.ToTable(nameof(UserCurrency));
        builder.HasKey(x => x.Id);
    }
}
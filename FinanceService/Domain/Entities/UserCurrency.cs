namespace FinanceService.Domain.Entities;

public class UserCurrency
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CurrencyId { get; set; }
}
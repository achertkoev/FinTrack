﻿namespace MigrationService.Models;

public class UserCurrency
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CurrencyId { get; set; }
}
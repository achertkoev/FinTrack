using System.Security.Claims;
using FinanceService.Contracts;
using FinanceService.Domain.Entities;
using FinanceService.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace FinanceService.Apis;

public static class FinanceApi
{
    public static IEndpointRouteBuilder MapFinanceApi(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/currencies");
        api.MapGet("", GetCurrenciesAsync)
            .WithDescription("Получить валюты");

        var favorites = api.MapGroup("favorites");
        
        favorites.MapPost("", CreateAsync)
            .RequireAuthorization()
            .WithDescription("Добавить валюту в избранное");
        
        favorites.MapGet("", GetMyFavoriteCurrenciesAsync)
            .RequireAuthorization()
            .WithDescription("Получить мои избранные валюты ");
        
        return api;
    }

    private static async Task<Results<Created, BadRequest<ProblemHttpResult>>> CreateAsync(
        [AsParameters] ServiceManager serviceManager, HttpContext context, FavoriteCreateDto favoriteCreateDto)
    {
        var currency = await serviceManager.Context.Currencies
            .FindAsync(favoriteCreateDto.CurrencyId);
        
        if (currency == null)
        {
            return TypedResults.BadRequest(TypedResults.Problem(title: "Валюта не найдена", 
                detail: "Укажите корректный CurrencyId"));
        }
        
        var userId = int.Parse(context.User.FindFirstValue(ClaimTypes.NameIdentifier));
        
        var existsUserCurrency = await serviceManager.Context.UserCurrencies
            .Where(x => x.UserId == userId)
            .Where(x => x.CurrencyId == favoriteCreateDto.CurrencyId)
            .FirstOrDefaultAsync();
        
        if (existsUserCurrency != null)
        {
            return TypedResults.BadRequest(TypedResults.Problem(title:"Валюта уже в избранном"));
        }

        var userCurrency = new UserCurrency()
        {
            UserId = userId,
            CurrencyId = favoriteCreateDto.CurrencyId,
        };
        await serviceManager.Context.UserCurrencies.AddAsync(userCurrency);
        await serviceManager.Context.SaveChangesAsync();

        return TypedResults.Created();
    }

    private static async Task<Ok<IEnumerable<CurrencyDto>>> GetCurrenciesAsync([AsParameters] ServiceManager serviceManager)
    {
        var currencies = await serviceManager.Context.Currencies
            .ToListAsync();
        
        var result = currencies.Select(x => new CurrencyDto()
        {
            Id = x.Id,
            Name = x.Name,
            Code = x.Code,
            Rate = x.Rate
        });
        
        return TypedResults.Ok(result);
    }
    
    private static async Task<Ok<IEnumerable<CurrencyDto>>> GetMyFavoriteCurrenciesAsync(
        [AsParameters] ServiceManager serviceManager, HttpContext context)
    {
        var userId = int.Parse(context.User.FindFirstValue(ClaimTypes.NameIdentifier));
        
        var queryCurrencyIds = serviceManager.Context.UserCurrencies
            .Where(x => x.UserId == userId)
            .Select(x => x.CurrencyId);
        
        var currencies = await serviceManager.Context.Currencies
            .Where(x=> queryCurrencyIds.Contains(x.Id))
            .ToListAsync();
        
        var result = currencies.Select(x => new CurrencyDto
        {
            Id = x.Id,
            Name = x.Name,
            Code = x.Code,
            Rate = x.Rate
        });
        
        return TypedResults.Ok(result);
    }
}
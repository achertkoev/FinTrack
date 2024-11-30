using System.Text;
using System.Xml;
using BackgroundService.Domain.Entities;
using BackgroundService.Infrastructure.Persistence;

namespace BackgroundService;
using Microsoft.Extensions.Hosting;

public class CurrencyUpdateService(IServiceProvider serviceProvider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var dailyExchangeRates = await GetDailyExchangeRates(stoppingToken);
            await CreateOrUpdateAsync(dailyExchangeRates, stoppingToken);
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
        
    }

    private async Task<IEnumerable<Currency>> GetDailyExchangeRates(CancellationToken stoppingToken)
    {
        using var scope = serviceProvider.CreateScope();
        var httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
        var httpClient = httpClientFactory.CreateClient();
        httpClient.BaseAddress = new Uri("https://www.cbr.ru/");
        var response = await httpClient.GetByteArrayAsync("scripts/XML_daily.asp", stoppingToken);
        var dailyExchangeRatesStr =  Encoding.GetEncoding("windows-1251").GetString(response);

        var currencies = new List<Currency>();
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(dailyExchangeRatesStr);
                
        var currencyNodes = xmlDoc.SelectNodes("//Valute");
        if (currencyNodes == null)
        {
            return [];
        }
        
        foreach (XmlNode currencyNode in currencyNodes)
        {
            var currency = new Currency
            {
                Id = int.Parse(currencyNode["NumCode"].InnerText),
                Name = currencyNode["Name"].InnerText,
                Code = currencyNode["CharCode"].InnerText,
                Rate = decimal.Parse(currencyNode["Value"].InnerText.Replace('.', ','))
            };
            currencies.Add(currency);
        }

        return currencies;
    }

    private async Task CreateOrUpdateAsync(IEnumerable<Currency> currencies, CancellationToken stoppingToken)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        foreach (var currency in currencies)
        {
            var existingCurrency = await dbContext.Currencies.FindAsync(currency.Id, stoppingToken);
            if (existingCurrency != null)
            {
                existingCurrency.Rate = currency.Rate;
            }
            else
            {
                await dbContext.Currencies.AddAsync(currency, stoppingToken);
            }
        }

        await dbContext.SaveChangesAsync(stoppingToken);
    }
}
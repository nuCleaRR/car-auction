using System.Text.Json;
using MongoDB.Driver;
using MongoDB.Entities;
using Polly;
using Polly.Extensions.Http;

public static class DbInitializer
{
    public async static Task Init(WebApplication app)
    {
        await Policy.Handle<TimeoutException>()
            .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(10))
            .ExecuteAndCaptureAsync(async () => await InitInternal(app));
    }

    private async static Task InitInternal(WebApplication app)
    {
        await DB.InitAsync("SearchDb",
            MongoClientSettings.FromConnectionString(app.Configuration.GetConnectionString("MongoDbConnection")));

        await DB.Index<Item>()
            .Key(x => x.Make, KeyType.Text)
            .Key(x => x.Model, KeyType.Text)
            .Key(x => x.Color, KeyType.Text)
            .CreateAsync();

        var count = await DB.CountAsync<Item>();

        using var scope = app.Services.CreateScope();
        var httpClient = scope.ServiceProvider.GetRequiredService<AuctionServiceHttpClient>();

        var items = await httpClient.GetItemsForSearchDb();
        Console.WriteLine(items.Count + " was returned from auction service");

        if (items.Count > 0)
        {
            await DB.SaveAsync(items);
        }
    }
}

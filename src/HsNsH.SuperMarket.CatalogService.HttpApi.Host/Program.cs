using System.Diagnostics.CodeAnalysis;
using HsNsH.SuperMarket.CatalogService.Domain.Models;
using HsNsH.SuperMarket.CatalogService.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace HsNsH.SuperMarket.CatalogService;

public static class Program
{
    [ExcludeFromCodeCoverage]
    public static async Task<int> Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        using (var scope = host.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<CatalogServiceDbContext>();
            await context.Database.MigrateAsync();
            if (!context.Categories.Any())
            {
                await context.Categories.AddRangeAsync(new List<Category>()
                {
                    new() { Name = "Default Category A" },
                    new() { Name = "Default Category B" },
                    new() { Name = "Default Category C" },
                    new() { Name = "Default Category D" },
                    new() { Name = "Default Category E" }
                });
                await context.SaveChangesAsync();
            }
        }

        await host.RunAsync();

        return 0;
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}
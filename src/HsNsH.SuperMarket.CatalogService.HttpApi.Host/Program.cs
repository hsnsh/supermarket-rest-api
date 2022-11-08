using System.Diagnostics.CodeAnalysis;

namespace HsNsH.SuperMarket.CatalogService;

public static class Program
{
    [ExcludeFromCodeCoverage]
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}
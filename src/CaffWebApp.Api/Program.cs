using CaffWebApp.Api;
using CaffWebApp.Api.Authentication;
using CaffWebApp.Api.Identity;
using CaffWebApp.Api.ProblemDetails;
using CaffWebApp.Api.StaticFile;
using CaffWebApp.Api.Swagger;
using CaffWebApp.BLL;
using CaffWebApp.DAL;
using Hellang.Middleware.ProblemDetails;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace CaffWebApp.Api;
public class Program
{
    public static void Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
               .Build();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        try
        {
            Log.Information("Starting web host");
            CreateHostBuilder(args).Build().Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Unhandled exception");
        }
        finally
        {
            Log.Information("Shut down complete");
            Log.CloseAndFlush();
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            })
            .UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));
}
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

try
{
    Log.Logger = new LoggerConfiguration().CreateBootstrapLogger();

    Log.Information("Starting up");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));

    builder.Services.AddDbContext<CaffDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")!));

    builder.Services.AddCaffBll(builder.Configuration);

    builder.Services.AddCaffWebAppIdentity();
    builder.Services.AddCaffWebAppIdentityServer();
    builder.Services.AddCaffWebAppAuthentication(builder.Configuration);

    builder.Services.AddRazorPages();
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddCaffWebAppSwagger(builder.Configuration);
    builder.Services.AddCaffWebAppProblemDetails();
    builder.Services.AddHttpContextAccessor();

    var app = builder.Build();

    app.UseSerilogRequestLogging();

    if (app.Environment.IsDevelopment())
    {
        app.UseCaffWebAppSwagger();
    }

    app.UseProblemDetails();
    app.UseHttpsRedirection();
    app.UseCaffStaticFiles();
    app.UseRouting();
    app.UseAuthentication();
    app.UseIdentityServer();
    app.UseAuthorization();

    app.MapRazorPages();
    app.MapControllers()
        .RequireAuthorization(AuthenticationExtensions.DefaultApiPolicy);

    app.Run();
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

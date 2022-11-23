using CaffWebApp.Api.Identity;
using CaffWebApp.Api.Options;
using CaffWebApp.Api.Swagger;
using CaffWebApp.BLL;
using CaffWebApp.DAL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Configuration;

namespace CaffWebApp.Api;

internal static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddRazorPages();

        builder.Services.AddDbContext<CaffDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")!));

        builder.Services.AddCaffWebAppIdentity();
        builder.Services.AddCaffWebAppIdentityServer();

        var caffApiOptions = builder.Configuration.GetSection(nameof(CaffWebApiOptions)).Get<CaffWebApiOptions>()!;
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Audience = caffApiOptions.Audience;
                options.Authority = caffApiOptions.Authority;
            });
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("ApiScope", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("scope", "api1");
            });
        });


        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddCaffWebAppSwagger(builder.Configuration);

        builder.Services.AddCaffBll();

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment())
        {
            app.UseCaffWebAppSwagger();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseIdentityServer();
        app.UseAuthorization();

        app.MapRazorPages();
        app.MapControllers();

        return app;
    }
}

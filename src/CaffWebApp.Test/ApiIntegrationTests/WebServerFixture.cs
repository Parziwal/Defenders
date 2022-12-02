using CaffWebApp.DAL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;

namespace CaffWebApp.Test.ApiIntegrationTests;

public class WebServerFixture : ApplicationDbContextInit, IDisposable
{
    public TestServer apiServer;

    public CaffDbContext DbContext { get; }
    private IDbContextTransaction transaction;

    public WebServerFixture()
    {
        apiServer = new TestServer(CreateWebAPIWebHostBuilder());
        apiServer.BaseAddress = new Uri(Configuration.GetSection("WebServer:WebAPIUrl").Value);

        DbContext = apiServer.Host.Services.GetRequiredService<CaffDbContext>();
        transaction = DbContext.Database.BeginTransaction();
    }

    private IWebHostBuilder CreateWebAPIWebHostBuilder()
    {
        return new WebHostBuilder()
            .UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration))
            .UseEnvironment(Environments.Development)
            .UseConfiguration(Configuration)
            .UseWebRoot(Configuration.GetSection("CaffWebApp:WebRoot").Value)
            .UseStartup<CaffWebApp.Api.Startup>()
            .ConfigureServices(services =>
            {
                var dbDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<CaffDbContext>));
                services.Remove(dbDescriptor);

                services.AddDbContext<CaffDbContext>(options =>
                {
                    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
                }, ServiceLifetime.Singleton, ServiceLifetime.Singleton);


                services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        SignatureValidator = (token, parameters) => new JwtSecurityToken(token)
                    };
                    options.TokenValidationParameters.ValidateAudience = false;
                    options.Authority = Configuration.GetSection("CaffWebApp:Authority").Value;
                    options.BackchannelHttpHandler = new MockBackChannel();
                    options.MetadataAddress = Configuration.GetSection("CaffWebApp:MetadataAddress").Value;
                });
            });
    }

    public void Dispose()
    {
        if (transaction != null)
        {
            transaction.Rollback();
            transaction.Dispose();
        }
    }
}

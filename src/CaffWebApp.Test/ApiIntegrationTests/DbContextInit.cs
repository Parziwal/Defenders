using CaffWebApp.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaffWebApp.Test.ApiIntegrationTests;

public class ApplicationDbContextInit
{
    protected static IConfiguration Configuration { get; }

    private static IConfiguration GetConfiguration()
    {
        return new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
    }

    static ApplicationDbContextInit()
    {
        Configuration = GetConfiguration();
        CreateDatabase();
    }

    private static void CreateDatabase()
    {
        var contextOptions = new DbContextOptionsBuilder<CaffDbContext>()
            .UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))
            .Options;

        var dbContext = new CaffDbContext(contextOptions);
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();
    }
}

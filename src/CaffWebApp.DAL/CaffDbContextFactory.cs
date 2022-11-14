using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CaffWebApp.DAL;

public class CaffDbContextFactory : IDesignTimeDbContextFactory<CaffDbContext>
{
    CaffDbContext IDesignTimeDbContextFactory<CaffDbContext>.CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<CaffDbContext>();
        optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

        return new CaffDbContext(optionsBuilder.Options);
    }
}

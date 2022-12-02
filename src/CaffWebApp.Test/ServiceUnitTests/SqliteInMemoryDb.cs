using CaffWebApp.DAL;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Data.Common;


namespace CaffWebApp.Test.ServiceUnitTests;

public class SqliteInMemoryDb : IDisposable
{
    private DbConnection connection;

    public SqliteInMemoryDb()
    {
        connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        using (var dbContext = CreateDbContext())
        {
            dbContext.Database.EnsureCreated();
        }
    }

    public CaffDbContext CreateDbContext()
    {
        var contextOptions = new DbContextOptionsBuilder<CaffDbContext>()
                        .UseSqlite(connection).Options;
        return new CaffDbContext(contextOptions);
    }

    public void Dispose()
    {
        connection.Dispose();
    }
}

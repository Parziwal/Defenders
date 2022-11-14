using CaffWebApp.DAL.Entites;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CaffWebApp.DAL;

public class CaffDbContext : IdentityDbContext<ApplicationUser>
{
    public CaffDbContext(DbContextOptions<CaffDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}

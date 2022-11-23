using CaffWebApp.DAL.Entites;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CaffWebApp.DAL;

public class CaffDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Caff> CaffImages => Set<Caff>();
    public DbSet<Ciff> CiffImages => Set<Ciff>();
    public DbSet<Comment> Comments => Set<Comment>();

    public CaffDbContext(DbContextOptions<CaffDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}

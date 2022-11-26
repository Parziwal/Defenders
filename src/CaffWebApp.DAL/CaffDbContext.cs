using CaffWebApp.DAL.Entites;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

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

        builder.Entity<ApplicationUser>().HasQueryFilter(user => !user.IsDeleted);
        builder.Entity<ApplicationUser>().HasMany(user => user.CaffImages).WithOne(caff => caff.UploadedBy).IsRequired(false);
        builder.Entity<ApplicationUser>().HasMany(user => user.Comments).WithOne(comment => comment.CreatedBy).IsRequired(false);

        builder.Entity<Comment>()
            .HasOne(c => c.CreatedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<IdentityRole>().HasData(
            new IdentityRole
            {
                Id = "rc95a82e-0abc-4d85-9877-4184177c3a7f",
                Name = Entites.UserRoles.Default,
                NormalizedName = Entites.UserRoles.Default.ToUpper(),
                ConcurrencyStamp = "e388975f-eb14-4f40-ba09-159e4164b513",
            },
            new IdentityRole
            {
                Id = "g8aceb4d-b534-459e-8c4e-d13374f43b65",
                Name = Entites.UserRoles.Admin,
                NormalizedName = Entites.UserRoles.Admin.ToUpper(),
                ConcurrencyStamp = "24d76572-e1bb-4588-b442-b3907c67e05e",
            }
        );

        builder.Entity<ApplicationUser>().HasData(
            new ApplicationUser
            {
                Id = "475c5e32-049c-4d7b-a963-02ebdc15a94b",
                Fullname = "Admin",
                UserName = "admin@email.hu",
                NormalizedUserName = "ADMIN@EMAIL.HU",
                Email = "admin@email.hu",
                NormalizedEmail = "ADMIN@EMAIL.HU",
                EmailConfirmed = true,
                PasswordHash = "AQAAAAEAACcQAAAAEAYhQeew7rP4OrFaPD7hY14miQgE+SY2grNxQ01VBp/7AGxnUtJLFZxVj+KLYk/2Rw==", //== Test.54321
                SecurityStamp = "QWHJ3YA4ZZ7PH7QGYAB2IU7PLUCA3LBO",
                ConcurrencyStamp = "70ceb6e6-9a79-4fb8-b325-93453e2021b1"
            }
        );

        builder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string>
            {
                UserId = "475c5e32-049c-4d7b-a963-02ebdc15a94b",
                RoleId = "g8aceb4d-b534-459e-8c4e-d13374f43b65"
            }
        );
    }
}

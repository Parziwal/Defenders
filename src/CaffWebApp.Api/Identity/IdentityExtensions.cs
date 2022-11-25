using CaffWebApp.DAL;
using CaffWebApp.DAL.Entites;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace CaffWebApp.Api.Identity;

public static class IdentityExtensions
{
    public static IdentityBuilder AddCaffWebAppIdentity(this IServiceCollection services) =>
        services.AddIdentity<ApplicationUser, IdentityRole>(opt =>
        {
            opt.Password.RequireDigit = true;
            opt.Password.RequireLowercase = true;
            opt.Password.RequireNonAlphanumeric = true;
            opt.Password.RequireUppercase = true;
            opt.Password.RequiredLength = 8;

            opt.SignIn.RequireConfirmedEmail = false;
        })
        .AddEntityFrameworkStores<CaffDbContext>()
        .AddDefaultTokenProviders();
    
    public static IIdentityServerBuilder AddCaffWebAppIdentityServer(this IServiceCollection services) =>
        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Identity/Account/Login";
            options.LogoutPath = "/Identity/Account/Logout";
            options.AccessDeniedPath = "/Identity/Account/AccessDenied";
        })
        .AddIdentityServer(options =>
        {
            options.Events.RaiseErrorEvents = true;
            options.Events.RaiseInformationEvents = true;
            options.Events.RaiseFailureEvents = true;
            options.Events.RaiseSuccessEvents = true;
            options.EmitStaticAudienceClaim = true;
        })
        .AddInMemoryIdentityResources(Config.IdentityResources)
        .AddInMemoryApiScopes(Config.ApiScopes)
        .AddInMemoryApiResources(Config.ApiResources)
        .AddInMemoryClients(Config.Clients)
        .AddAspNetIdentity<ApplicationUser>()
        .AddProfileService<IdentityProfileService>();
}

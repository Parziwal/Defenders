using CaffWebApp.BLL.Options;
using CaffWebApp.DAL.Entites;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;

namespace CaffWebApp.Api.Authentication;

public static class AuthenticationExtensions
{
    public const string DefaultApiPolicy = "DefaultApiPolicy";

    public static IServiceCollection AddCaffWebAppAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var caffApiOptions = configuration.GetSection(nameof(CaffWebApiOptions)).Get<CaffWebApiOptions>()!;
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.Authority = caffApiOptions.BaseUrl;
            options.TokenValidationParameters.ValidateAudience = false;
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(DefaultApiPolicy, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("scope", "caffwebapp.api");
                policy.AuthenticationSchemes = new[] { JwtBearerDefaults.AuthenticationScheme };
            });

            options.AddPolicy(UserRoles.Admin, policy =>
            {
                
                policy.RequireClaim(ClaimTypes.Role, UserRoles.Admin);
            });

            options.DefaultPolicy = options.GetPolicy(DefaultApiPolicy)!;
        });

        return services;
    }
}

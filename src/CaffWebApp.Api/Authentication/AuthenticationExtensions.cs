using CaffWebApp.BLL.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace CaffWebApp.Api.Authentication;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddCaffWebAppAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var caffApiOptions = configuration.GetSection(nameof(CaffWebApiOptions)).Get<CaffWebApiOptions>()!;
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Audience = caffApiOptions.Audience;
                options.Authority = caffApiOptions.Authority;
                options.TokenValidationParameters.ValidTypes = new[] { "at+jwt" };
            });
        services.AddAuthorization(options =>
        {
            options.AddPolicy("Default", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("scope", "caffwebapp.api");
            });

            options.DefaultPolicy = options.GetPolicy("Default")!;
        });

        return services;
    }
}

using CaffWebApp.Api.Options;
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
            });
        services.AddAuthorization(options =>
        {
            options.AddPolicy("ApiScope", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("scope", "api1");
            });
        });

        return services;
    }
}

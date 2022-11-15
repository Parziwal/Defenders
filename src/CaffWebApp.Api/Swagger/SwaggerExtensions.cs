using CaffWebApp.Api.Options;
using Microsoft.AspNetCore.Authentication.OAuth;
using NSwag;
using NSwag.AspNetCore;
using NSwag.Generation.Processors.Security;

namespace CaffWebApp.Api.Swagger;

public static class SwaggerExtensions
{
    public static IServiceCollection AddCaffWebAppSwagger(this IServiceCollection services, IConfiguration configuration)
    {
        var caffApiOptions = configuration.GetSection(nameof(CaffWebApiOptions)).Get<CaffWebApiOptions>()!;
        services.AddOpenApiDocument(config =>
        {
            config.Title = "CaffWepApp Api";
            config.Version = "v1";
            
            config.AddSecurity("OAuth2", new OpenApiSecurityScheme
            {
                Type = OpenApiSecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = $"{caffApiOptions.Authority}/connect/authorize",
                        TokenUrl = $"{caffApiOptions.Authority}/connect/token",
                        Scopes = new Dictionary<string, string> { {caffApiOptions.ApiScope , caffApiOptions.ApiScopeDisplayName } },
                    },
                },
            });

            config.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("OAuth2"));
        });

        return services;
    }

    public static WebApplication UseCaffWebAppSwagger(this WebApplication app)
    {
        var caffApiOptions = app.Configuration.GetSection(nameof(CaffWebApiOptions)).Get<CaffWebApiOptions>()!;
        app.UseOpenApi();
        app.UseSwaggerUi3(settings =>
        {
            settings.OAuth2Client = new OAuth2ClientSettings
            {
                ClientId = caffApiOptions.SwaggerClientId,
                UsePkceWithAuthorizationCodeGrant = true,
            };
            settings.OAuth2Client.Scopes.Add(caffApiOptions.ApiScope);
        });

        return app;
    }
}

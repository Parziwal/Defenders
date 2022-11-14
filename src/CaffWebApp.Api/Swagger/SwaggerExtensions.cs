using NSwag;
using NSwag.AspNetCore;
using NSwag.Generation.Processors.Security;

namespace CaffWebApp.Api.Swagger;

public static class SwaggerExtensions
{
    public static IServiceCollection AddCaffWebAppSwagger(this IServiceCollection services, IConfiguration configuration)
    {
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
                        AuthorizationUrl = $"https://localhost:5001/connect/authorize",
                        TokenUrl = $"https://localhost:5001/connect/token",
                        Scopes = new Dictionary<string, string> { { "caffwebapp.api", "CaffWebApp Api Swagger access" } },
                    },
                },
            });

            config.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("OAuth2"));
        });

        return services;
    }

    public static IApplicationBuilder UseCaffWebAppSwagger(this IApplicationBuilder app)
    {
        app.UseOpenApi();
        app.UseSwaggerUi3(settings =>
        {
            settings.OAuth2Client = new OAuth2ClientSettings
            {
                ClientId = "caff.swagger.client",
                UsePkceWithAuthorizationCodeGrant = true,
            };
            settings.OAuth2Client.Scopes.Add("caffwebapp.api");
        });

        return app;
    }
}

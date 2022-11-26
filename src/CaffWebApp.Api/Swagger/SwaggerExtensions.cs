﻿using CaffWebApp.BLL.Options;
using Microsoft.Extensions.Options;
using NJsonSchema.Generation;
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
            config.DocumentName = "CaffWepApp";
            config.Title = "CaffWepApp Api";
            config.Version = "v1";
            config.DefaultReferenceTypeNullHandling = ReferenceTypeNullHandling.NotNull;
            config.DefaultResponseReferenceTypeNullHandling = ReferenceTypeNullHandling.NotNull;

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

    public static IApplicationBuilder UseCaffWebAppSwagger(this IApplicationBuilder app)
    {
        var caffApiOptions = app.ApplicationServices.GetService<IOptions<CaffWebApiOptions>>()!.Value;
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

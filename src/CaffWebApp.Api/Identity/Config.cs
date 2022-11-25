using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace CaffWebApp.Api.Identity;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email(),
            new IdentityResource("role", new [] { "role" })
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new List<ApiScope>
        {
            new ApiScope("caffwebapp.api", "CaffWebApp API")
        };

    public static IEnumerable<ApiResource> ApiResources =>
        new List<ApiResource>
        {
            new ApiResource("caffwebapp.api", "CaffWebApp API", new List<string>() { "caffwebapp.api" })
        };

    public static IEnumerable<Client> Clients =>
        new List<Client>
        {
            new Client
            {
                ClientId = "caff.angular.client",
                ClientName = "Caff Angular Client",
                AllowedGrantTypes = GrantTypes.Code,
                RequireClientSecret = false,
                RequirePkce = true,
                RequireConsent = false,
                AccessTokenLifetime = 3600,
                RedirectUris = { "http://localhost:4200", "https://localhost:4200" },
                PostLogoutRedirectUris = { "http://localhost:4200", "https://localhost:4200" },
                AllowedCorsOrigins = { "http://localhost:4200", "https://localhost:4200" },

                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    "role",
                    "caffwebapp.api"
                },
                AlwaysIncludeUserClaimsInIdToken = true,
            },
             new Client
            {
                ClientId = "caff.swagger.client",
                ClientName = "Caff Swagger Client",
                AllowedGrantTypes = GrantTypes.Code,
                RequireClientSecret = false,
                RequirePkce = true,
                RequireConsent = false,
                AccessTokenLifetime = 3600,
                RedirectUris = { "https://localhost:5001/swagger/oauth2-redirect.html" },
                AllowedCorsOrigins = { "https://localhost:5001" },

                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    "caffwebapp.api"
                }
            }
        };
}
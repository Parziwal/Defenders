using CaffWebApp.DAL.Entites;
using Duende.IdentityServer;
using Duende.IdentityServer.AspNetIdentity;
using Duende.IdentityServer.Models;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace CaffWebApp.Api.Identity
{
    public class IdentityProfileService : ProfileService<ApplicationUser>
    {
        public IdentityProfileService(UserManager<ApplicationUser> userManager, IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory) : base(userManager, claimsFactory)
        {
        }

        public override async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var user = await UserManager.GetUserAsync(context.Subject);
            var principal = await GetUserClaimsAsync(user);
            var id = (ClaimsIdentity)principal.Identity!;
            IList<string> roles = await UserManager.GetRolesAsync(user);

            Claim nameClaim = id.FindFirst(JwtClaimTypes.Name)!;
            id.RemoveClaim(nameClaim);
            id.AddClaim(new Claim(JwtClaimTypes.Name, user.Fullname));
            id.AddClaim(new Claim(IdentityServerConstants.StandardScopes.Email, user.Email));
            foreach (string role in roles)
            {
                id.AddClaim(new Claim(JwtClaimTypes.Role, role));
            }

            context.AddRequestedClaims(principal.Claims);
        }
    }
}


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
            IList<string> roles = await UserManager.GetRolesAsync(user);

            var claims = new List<Claim>();
            claims.Add(new Claim(JwtClaimTypes.Name, user.Fullname));
            claims.Add(new Claim(JwtClaimTypes.Email, user.Email));
            foreach (string role in roles)
            {
                claims.Add(new Claim(JwtClaimTypes.Role, role));
            }

            context.IssuedClaims.AddRange(claims);
        }
    }
}


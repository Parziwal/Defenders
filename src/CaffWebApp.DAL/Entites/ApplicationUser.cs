using Microsoft.AspNetCore.Identity;

namespace CaffWebApp.DAL.Entites;

public class ApplicationUser : IdentityUser
{
    public string Fullname { get; set; } = default!;
}
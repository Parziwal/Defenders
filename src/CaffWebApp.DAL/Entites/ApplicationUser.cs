using Microsoft.AspNetCore.Identity;

namespace CaffWebApp.DAL.Entites;

public class ApplicationUser : IdentityUser
{
    required public string Fullname { get; set; }
}
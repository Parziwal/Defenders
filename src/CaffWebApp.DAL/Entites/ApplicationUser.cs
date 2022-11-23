using Microsoft.AspNetCore.Identity;

namespace CaffWebApp.DAL.Entites;

public class ApplicationUser : IdentityUser
{
    public string Fullname { get; set; } = default!;
    public ICollection<Caff> CaffImages { get; set; } = default!;
    public ICollection<Comment> Comments { get; set; } = default!;
}
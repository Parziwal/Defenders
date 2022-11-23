namespace CaffWebApp.BLL.Dtos.User;

public class UserDto
{
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;

    public UserDto(DAL.Entites.ApplicationUser entity)
    {
        FullName = entity.Fullname;
        Email = entity.Email;
    }
}

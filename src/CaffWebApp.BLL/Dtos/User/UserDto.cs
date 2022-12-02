namespace CaffWebApp.BLL.Dtos.User;

public class UserDto
{
    public string Id { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;

    public UserDto() { }

    public UserDto(DAL.Entites.ApplicationUser entity)
    {
        Id = entity.Id;
        FullName = entity.Fullname;
        Email = entity.Email;
    }
}

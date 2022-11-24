namespace CaffWebApp.BLL.Dtos.User;

public class UserDto
{
    public string Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }

    public UserDto(DAL.Entites.ApplicationUser entity)
    {
        Id = entity.Id;
        FullName = entity.Fullname;
        Email = entity.Email;
    }
}

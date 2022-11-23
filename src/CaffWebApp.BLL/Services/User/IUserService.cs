using CaffWebApp.BLL.Dtos.User;

namespace CaffWebApp.BLL.Services.User;

public interface IUserService
{
    Task<List<UserDto>> ListAllUsers();
    Task DeleteUser(int userId);
}

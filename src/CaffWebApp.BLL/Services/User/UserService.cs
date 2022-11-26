using CaffWebApp.BLL.Dtos.User;
using CaffWebApp.BLL.Exceptions;
using CaffWebApp.BLL.Extensions;
using CaffWebApp.DAL.Entites;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace CaffWebApp.BLL.Services.User;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHttpContextAccessor _httpContext;

    public UserService(
        UserManager<ApplicationUser> userManager,
        IHttpContextAccessor httpContext)
    {
        _userManager = userManager;
        _httpContext = httpContext;
    }

    public async Task DeleteUser(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null || user.Id != _httpContext.GetCurrentUserId())
        {
             throw new EntityNotFoundException($"User with {userId} id does not exists!");
        }
        
        user.IsDeleted = true;
        await _userManager.UpdateAsync(user);
    }

    public async Task<List<UserDto>> ListAllUsers() =>
        (await _userManager.GetUsersInRoleAsync(UserRoles.Default))
                .Select(user => new UserDto(user))
                .ToList();
}

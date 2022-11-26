using CaffWebApp.BLL.Dtos.User;
using CaffWebApp.BLL.Exceptions;
using CaffWebApp.BLL.Extensions;
using CaffWebApp.DAL;
using CaffWebApp.DAL.Entites;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CaffWebApp.BLL.Services.User;

public class UserService : IUserService
{
    private readonly CaffDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHttpContextAccessor _httpContext;

    public UserService(
        CaffDbContext dbContext,
        UserManager<ApplicationUser> userManager,
        IHttpContextAccessor httpContext)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _httpContext = httpContext;
    }

    public async Task DeleteUser(string userId)
    {
        var user = await _dbContext.Users
                    .SingleOrDefaultAsync(user => user.Id == userId && user.Id != _httpContext.GetCurrentUserId());

        if (user == null)
        {
             throw new EntityNotFoundException($"User with {userId} id does not exists!");
        }

        user.IsDeleted = true;
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<UserDto>> ListAllUsers() =>
        (await _userManager.GetUsersInRoleAsync(UserRoles.Default))
                .Select(user => new UserDto(user))
                .ToList();
}

using CaffWebApp.BLL.Dtos.User;
using CaffWebApp.BLL.Exceptions;
using CaffWebApp.DAL;
using CaffWebApp.DAL.Entites;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CaffWebApp.BLL.Services.User;

public class UserService : IUserService
{
    private readonly CaffDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(CaffDbContext dbContext, UserManager<ApplicationUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task DeleteUser(string userId)
    {
        var user = await _dbContext.Users
                    .SingleOrDefaultAsync(user => user.Id == userId);

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

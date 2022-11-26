using CaffWebApp.BLL.Dtos.User;
using CaffWebApp.BLL.Services.User;
using CaffWebApp.DAL.Entites;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CaffWebApp.Api.Controllers
{
    [Authorize(Policy = UserRoles.Admin)]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public Task<List<UserDto>> ListAllUsers() =>
            _userService.ListAllUsers();

        [HttpDelete("{userId}")]
        public Task DeleteUser(string userId) =>
            _userService.DeleteUser(userId);
    }
}

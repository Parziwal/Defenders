using CaffWebApp.BLL.Dtos.User;
using CaffWebApp.BLL.Services.User;
using Microsoft.AspNetCore.Mvc;

namespace CaffWebApp.Api.Controllers
{
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

        [HttpPut("{userId}")]
        public Task DeleteUser(string userId) =>
            _userService.DeleteUser(userId);
    }
}

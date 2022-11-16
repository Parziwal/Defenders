using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CaffWebApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpGet]
        public Task ListAllUsers()
        {
            return default!;
        }

        [HttpPut("{userId}")]
        public Task DeleteUser(int userId)
        {
            return default!;
        }
    }
}

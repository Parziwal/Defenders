using CaffWebApp.BLL.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CaffWebApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        [HttpPost("{caffId}")]
        public Task AddCommentToCaff(int caffId, [FromBody] AddOrEditCommentDto commentDto)
        {
            return default!;
        }

        [HttpPut("{commentId}")]
        public Task EditComment(int commentId, [FromBody] AddOrEditCommentDto commentDto)
        {
            return default!;
        }

        [HttpDelete("{commentId}")]
        public Task DeleteComment(int commentId)
        {
            return default!;
        }
    }
}

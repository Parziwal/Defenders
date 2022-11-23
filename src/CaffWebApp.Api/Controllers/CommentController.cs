using CaffWebApp.BLL.Dtos.Comment;
using CaffWebApp.BLL.Services.Comment;
using Microsoft.AspNetCore.Mvc;

namespace CaffWebApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpPost("caff/{caffId}")]
        public Task<CommentDto> AddCommentToCaff(int caffId, [FromBody] AddOrEditCommentDto commentDto) =>
            _commentService.AddCommentToCaff(caffId, commentDto);

        [HttpPut("{commentId}")]
        public Task<CommentDto> EditComment(int commentId, [FromBody] AddOrEditCommentDto commentDto) =>
            _commentService.EditComment(commentId, commentDto);

        [HttpDelete("{commentId}")]
        public Task DeleteComment(int commentId) =>
            _commentService.DeleteComment(commentId);
    }
}

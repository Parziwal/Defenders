using CaffWebApp.BLL.Dtos.Comment;

namespace CaffWebApp.BLL.Services.Comment;

public interface ICommentService
{
    Task<CommentDto> AddCommentToCaff(int caffId, AddOrEditCommentDto commentDto);
    Task<CommentDto> EditComment(int commentId, AddOrEditCommentDto commentDto);
    Task DeleteComment(int commentId);
}

using CaffWebApp.BLL.Dtos.Comment;

namespace CaffWebApp.BLL.Services.Comment;

public class CommentService : ICommentService
{
    public Task<CommentDto> AddCommentToCaff(int caffId, AddOrEditCommentDto commentDto)
    {
        throw new NotImplementedException();
    }

    public Task DeleteComment(int commentId)
    {
        throw new NotImplementedException();
    }

    public Task<CommentDto> EditComment(int commentId, AddOrEditCommentDto commentDto)
    {
        throw new NotImplementedException();
    }
}

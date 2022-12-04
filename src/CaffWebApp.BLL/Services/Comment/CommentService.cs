using CaffWebApp.BLL.Dtos.Comment;
using CaffWebApp.BLL.Exceptions;
using CaffWebApp.BLL.Extensions;
using CaffWebApp.DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CaffWebApp.BLL.Services.Comment;

public class CommentService : ICommentService
{
    private readonly CaffDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContext;

    public CommentService(CaffDbContext dbContext, IHttpContextAccessor httpContext)
    {
        _dbContext = dbContext;
        _httpContext = httpContext;
    }

    public async Task<CommentDto> AddCommentToCaff(int caffId, AddOrEditCommentDto commentDto)
    {
        var caff = await _dbContext.CaffImages
                       .SingleOrDefaultAsync(caff => caff.Id == caffId);

        if (caff == null)
        {
            throw new EntityNotFoundException($"Comment with {caffId} id does not exists!");
        }

        var comment = new DAL.Entites.Comment()
        {
            CaffImage = caff,
            Text = commentDto.CommentText,
            CreatedBy = await _dbContext.Users
                       .SingleAsync(user => user.Id == _httpContext.GetCurrentUserId()),
            CreateAt = DateTimeOffset.Now,            
        };

        _dbContext.Add(comment);

        await _dbContext.SaveChangesAsync();

        return new CommentDto(comment);
    }

    public async Task<CommentDto> EditComment(int commentId, AddOrEditCommentDto commentDto)
    {
        var comment = await _dbContext.Comments
                       .Include(comment => comment.CreatedBy)
                       .SingleOrDefaultAsync(comment => comment.Id == commentId);

        if (comment == null)
        {
            throw new EntityNotFoundException($"Comment with {commentId} id does not exists!");
        }

        comment.Text = commentDto.CommentText;
        await _dbContext.SaveChangesAsync();

        return new CommentDto(comment);
    }

    public async Task DeleteComment(int commentId)
    {
        var comment = await _dbContext.Comments
                      .SingleOrDefaultAsync(comment => comment.Id == commentId);

        if (comment == null)
        {
            throw new EntityNotFoundException($"Comment with {commentId} id does not exists!");
        }

        _dbContext.Remove(comment);
        await _dbContext.SaveChangesAsync();
    }
}

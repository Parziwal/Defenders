using CaffWebApp.BLL.Dtos.Caff;
using CaffWebApp.BLL.Dtos.Comment;
using CaffWebApp.BLL.Dtos.Parser;
using CaffWebApp.BLL.Exceptions;
using CaffWebApp.BLL.Services.Caff;
using CaffWebApp.BLL.Services.Comment;
using CaffWebApp.BLL.Services.Parser;
using CaffWebApp.DAL.Entites;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Net.Mime;

namespace CaffWebApp.Test.ServiceUnitTests;

public class CommentServiceTest : SqliteInMemoryDb
{
    [Fact]
    public async void AddCommentToCaff()
    {
        using (var dbContext = CreateDbContext())
        {
            //Arrange
            var user = TestHelper.CreateUser();
            var sampleCaff = TestHelper.CreateCaff();
            dbContext.Add(user);
            dbContext.Add(sampleCaff);
            await dbContext.SaveChangesAsync();

            var sampleComment = new AddOrEditCommentDto()
            {
                CommentText = "NewUserComment",
            };

            var httpContext = new Mock<IHttpContextAccessor>();
            var commentService = new CommentService(dbContext, httpContext.Object);

            httpContext.SetupGet(h => h.HttpContext!.User)
                .Returns(TestHelper.GetUserClaimPrinciple(sampleCaff.UploadedBy.Id));

            //Act
            var result = await commentService.AddCommentToCaff(sampleCaff.Id, sampleComment);

            //Assert
            Assert.Equal(result.Text, sampleComment.CommentText);
            Assert.Equal(result.CreatedBy, user.Fullname);
        }
    }


    [Fact]
    public async void AddCommentToCaff_NotExistingCaff()
    {
        using (var dbContext = CreateDbContext())
        {
            //Arrange
            var httpContext = new Mock<IHttpContextAccessor>();
            var commentService = new CommentService(dbContext, httpContext.Object);

            //Act

            //Assert
            var exception = await Assert.ThrowsAsync<EntityNotFoundException>(
              async () => await commentService.AddCommentToCaff(-1, new AddOrEditCommentDto())
            );
        }
    }


    [Fact]
    public async void EditCommentToCaff_Existing()
    {
        using (var dbContext = CreateDbContext())
        {
            //Arrange
            var sampleCaff = TestHelper.CreateCaff();
            dbContext.Add(sampleCaff);
            await dbContext.SaveChangesAsync();

            var sampleComment = new AddOrEditCommentDto()
            {
                CommentText = "NewUserComment",
            };

            var httpContext = new Mock<IHttpContextAccessor>();
            var commentService = new CommentService(dbContext, httpContext.Object);

            //Act

            var result = await commentService.EditComment(sampleCaff.Id, sampleComment);

            //Assert
            Assert.Equal(result.Text, sampleComment.CommentText);
        }
    }

    [Fact]
    public async void EditCommentToCaff_NotExisting()
    {
        using (var dbContext = CreateDbContext())
        {
            //Arrange
            var httpContext = new Mock<IHttpContextAccessor>();
            var commentService = new CommentService(dbContext, httpContext.Object);

            //Act

            //Assert
            var exception = await Assert.ThrowsAsync<EntityNotFoundException>(
              async () => await commentService.EditComment(-1, new AddOrEditCommentDto())
            );
        }
    }

    [Fact]
    public async void DeleteComment_Existing()
    {
        using (var dbContext = CreateDbContext())
        {
            //Arrange
            var sampleCaff = TestHelper.CreateCaff();
            dbContext.Add(sampleCaff);
            await dbContext.SaveChangesAsync();

            var httpContext = new Mock<IHttpContextAccessor>();
            var commentService = new CommentService(dbContext, httpContext.Object);

            //Act
            var commentId = sampleCaff.Comments.First().Id;
            await commentService.DeleteComment(commentId);

            //Assert
            var result = await dbContext.Comments.SingleOrDefaultAsync(comment => comment.Id == commentId);
            Assert.Null(result);
        }
    }

    [Fact]
    public async void DeleteComment_NotExisting()
    {
        using (var dbContext = CreateDbContext())
        {
            //Arrange
            var httpContext = new Mock<IHttpContextAccessor>();
            var commentService = new CommentService(dbContext, httpContext.Object);

            //Act

            //Assert
            var exception = await Assert.ThrowsAsync<EntityNotFoundException>(
              async () => await commentService.DeleteComment(-1)
            );
        }
    }
}
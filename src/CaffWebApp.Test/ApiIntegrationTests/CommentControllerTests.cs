using CaffWebApp.BLL.Dtos.Caff;
using CaffWebApp.BLL.Dtos.Comment;
using CaffWebApp.BLL.Exceptions;
using CaffWebApp.BLL.Services.Comment;
using Duende.IdentityServer.Models;
using IdentityModel.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Net;
using System.Net.Http.Json;

namespace CaffWebApp.Test.ApiIntegrationTests;

public class CommentControllerTests : WebServerFixture
{
    [Fact]
    public async void AddCommentToCaff()
    {
        //Arrange
        var sampleCaff = TestHelper.CreateCaff();
        DbContext.Add(sampleCaff);
        await DbContext.SaveChangesAsync();

        var sampleComment = new AddOrEditCommentDto()
        {
            CommentText = "NewUserComment",
        };

        var accessToken = await apiServer.GetAccessToken("admin@email.hu", "Test.54321");
        var client = apiServer.CreateClient();
        client.SetBearerToken(accessToken);

        //Act
        var commentResponse = await client.PostAsJsonAsync($"api/comment/caff/{sampleCaff.Id}", sampleComment);
        var result = await commentResponse.Content.ReadFromJsonAsync<CommentDto>();

        //Assert
        Assert.Equal(HttpStatusCode.OK, commentResponse.StatusCode);
        Assert.Equal(sampleComment.CommentText, result!.Text);
        Assert.Equal("Admin", result.CreatedBy);
    }


    [Fact]
    public async void AddCommentToCaff_NotExistingCaff()
    {
        var sampleComment = new AddOrEditCommentDto()
        {
            CommentText = "NewUserComment",
        };

        var accessToken = await apiServer.GetAccessToken("admin@email.hu", "Test.54321");
        var client = apiServer.CreateClient();
        client.SetBearerToken(accessToken);

        //Act
        var commentResponse = await client.PostAsJsonAsync($"api/comment/caff/0", sampleComment);

        //Assert
        Assert.Equal(HttpStatusCode.NotFound, commentResponse.StatusCode);
    }


    [Fact]
    public async void EditCommentToCaff_Existing()
    {
        //Arrange
        var sampleCaff = TestHelper.CreateCaff();
        DbContext.Add(sampleCaff);
        await DbContext.SaveChangesAsync();

        var sampleComment = new AddOrEditCommentDto()
        {
            CommentText = "EditedUserComment",
        };

        var accessToken = await apiServer.GetAccessToken("admin@email.hu", "Test.54321");
        var client = apiServer.CreateClient();
        client.SetBearerToken(accessToken);

        //Act
        var commentResponse = await client.PutAsJsonAsync($"api/comment/{sampleCaff.Comments.First().Id}", sampleComment);
        var result = await commentResponse.Content.ReadFromJsonAsync<CommentDto>();

        //Assert
        Assert.Equal(HttpStatusCode.OK, commentResponse.StatusCode);
        Assert.Equal(sampleComment.CommentText, result!.Text);
    }

    [Fact]
    public async void EditCommentToCaff_NotExisting()
    {
        //Arrange
        var sampleComment = new AddOrEditCommentDto()
        {
            CommentText = "EditedUserComment",
        };

        var accessToken = await apiServer.GetAccessToken("admin@email.hu", "Test.54321");
        var client = apiServer.CreateClient();
        client.SetBearerToken(accessToken);

        //Act
        var commentResponse = await client.PutAsJsonAsync($"api/comment/0", sampleComment);

        //Assert
        Assert.Equal(HttpStatusCode.NotFound, commentResponse.StatusCode);
    }

    [Fact]
    public async void DeleteComment_Existing()
    {
        //Arrange
        var sampleCaff = TestHelper.CreateCaff();
        DbContext.Add(sampleCaff);
        await DbContext.SaveChangesAsync();

        var accessToken = await apiServer.GetAccessToken("admin@email.hu", "Test.54321");
        var client = apiServer.CreateClient();
        client.SetBearerToken(accessToken);

        //Act
        var commentId = sampleCaff.Comments.First().Id;
        var commentResponse = await client.DeleteAsync($"api/comment/{commentId}");

        //Assert
        Assert.Equal(HttpStatusCode.OK, commentResponse.StatusCode);
        var result = await DbContext.Comments.SingleOrDefaultAsync(comment => comment.Id == commentId);
        Assert.Null(result);
    }

    [Fact]
    public async void DeleteComment_NotExisting()
    {
        //Arrange
        var accessToken = await apiServer.GetAccessToken("admin@email.hu", "Test.54321");
        var client = apiServer.CreateClient();
        client.SetBearerToken(accessToken);

        //Act
        var commentResponse = await client.DeleteAsync($"api/comment/0");

        //Assert
        Assert.Equal(HttpStatusCode.NotFound, commentResponse.StatusCode);
    }

    [Fact]
    public async Task EditComment_NormalUser()
    {
        //Arrange
        var user = TestHelper.CreateUser();
        DbContext.Add(user);
        await DbContext.SaveChangesAsync();

        var accessToken = await apiServer.GetAccessToken(user.Email, "Test.54321");
        var client = apiServer.CreateClient();
        client.SetBearerToken(accessToken);

        var sampleComment = new AddOrEditCommentDto()
        {
            CommentText = "UserComment",
        };

        //Act
        var commentResponse = await client.PutAsJsonAsync($"api/comment/1", sampleComment);

        //Assert
        Assert.Equal(HttpStatusCode.Unauthorized, commentResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteComment_NormalUser()
    {
        //Arrange
        var user = TestHelper.CreateUser();
        DbContext.Add(user);
        await DbContext.SaveChangesAsync();

        var accessToken = await apiServer.GetAccessToken(user.Email, "Test.54321");
        var client = apiServer.CreateClient();
        client.SetBearerToken(accessToken);

        //Act
        var commentResponse = await client.DeleteAsync($"api/comment/1");

        //Assert
        Assert.Equal(HttpStatusCode.Unauthorized, commentResponse.StatusCode);
    }

    [Fact]
    public async Task ListCaffImages_NotRegisteredUser()
    {
        //Arrange
        var client = apiServer.CreateClient();
        var sampleComment = new AddOrEditCommentDto()
        {
            CommentText = "UserComment",
        };

        //Act
        var commentResponse = await client.PostAsJsonAsync($"api/comment/caff/1", sampleComment);

        //Assert
        Assert.Equal(HttpStatusCode.Unauthorized, commentResponse.StatusCode);
    }
}

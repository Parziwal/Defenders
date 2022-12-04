using CaffWebApp.BLL.Dtos.Comment;
using CaffWebApp.BLL.Exceptions;
using CaffWebApp.BLL.Services.Comment;
using CaffWebApp.BLL.Services.User;
using CaffWebApp.DAL.Entites;
using IdentityModel.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Duende.IdentityServer.Models;
using CaffWebApp.BLL.Dtos.User;

namespace CaffWebApp.Test.ApiIntegrationTests;

public class UserControllerTests : WebServerFixture
{
    [Fact]
    public async void DeleteUser_UserExists()
    {
        //Arrange
        var user = TestHelper.CreateUser();
        DbContext.Add(user);
        await DbContext.SaveChangesAsync();

        var accessToken = await apiServer.GetAccessToken("admin@email.hu", "Test.54321");
        var client = apiServer.CreateClient();
        client.SetBearerToken(accessToken);

        //Act
        var userId = user.Id;
        var userResponse = await client.DeleteAsync($"api/user/{userId}");

        //Assert
        Assert.Equal(HttpStatusCode.OK, userResponse.StatusCode);
        var result = await DbContext.Users.SingleOrDefaultAsync(user => user.Id == userId);
        Assert.True(result!.IsDeleted);
    }

    [Fact]
    public async void DeleteUser_UserNotExists()
    {
        //Arrange
        var accessToken = await apiServer.GetAccessToken("admin@email.hu", "Test.54321");
        var client = apiServer.CreateClient();
        client.SetBearerToken(accessToken);

        //Act
        var userResponse = await client.DeleteAsync($"api/user/0");

        //Assert
        Assert.Equal(HttpStatusCode.NotFound, userResponse.StatusCode);
    }


    [Fact]
    public async void ListAllUsers()
    {
        //Arrange
        var user = TestHelper.CreateUser();
        DbContext.Add(user);
        await DbContext.SaveChangesAsync();

        var role = await DbContext.Roles.SingleAsync(role => role.Name == "Default");
        DbContext.UserRoles.Add(new IdentityUserRole<string>() { RoleId = role.Id, UserId = user.Id });
        await DbContext.SaveChangesAsync();

        var accessToken = await apiServer.GetAccessToken("admin@email.hu", "Test.54321");
        var client = apiServer.CreateClient();
        client.SetBearerToken(accessToken);

        //Act
        var userResponse = await client.GetAsync($"api/user");
        var result = await userResponse.Content.ReadFromJsonAsync<List<UserDto>>();

        //Assert
        Assert.Equal(HttpStatusCode.OK, userResponse.StatusCode);
        Assert.Single(result!);
        Assert.Equal(user.Email, result[0].Email);
    }

    [Fact]
    public async void ListAllUsers_NormalUser()
    {
        //Arrange
        var user = TestHelper.CreateUser();
        DbContext.Add(user);
        await DbContext.SaveChangesAsync();

        var accessToken = await apiServer.GetAccessToken(user.Email, "Test.54321");
        var client = apiServer.CreateClient();
        client.SetBearerToken(accessToken);

        //Act
        var userResponse = await client.GetAsync($"api/user");

        //Assert
        Assert.Equal(HttpStatusCode.Unauthorized, userResponse.StatusCode);
    }

    [Fact]
    public async void ListAllUsers_UnRegisteredUser()
    {
        //Arrange
        var client = apiServer.CreateClient();

        //Act
        var userResponse = await client.GetAsync($"api/user");

        //Assert
        Assert.Equal(HttpStatusCode.Unauthorized, userResponse.StatusCode);
    }
}

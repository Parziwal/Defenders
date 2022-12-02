using CaffWebApp.BLL.Dtos.Caff;
using CaffWebApp.BLL.Dtos.Comment;
using CaffWebApp.BLL.Dtos.Parser;
using CaffWebApp.BLL.Exceptions;
using CaffWebApp.BLL.Services.Caff;
using CaffWebApp.BLL.Services.Comment;
using CaffWebApp.BLL.Services.Parser;
using CaffWebApp.BLL.Services.User;
using CaffWebApp.DAL.Entites;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Net.Mime;

namespace CaffWebApp.Test.ServiceUnitTests;

public class UserServiceTest : SqliteInMemoryDb
{
    [Fact]
    public async void DeleteUser_UserExists()
    {
        using (var dbContext = CreateDbContext())
        {
            //Arrange
            var user = TestHelper.CreateUser();

            var store = new Mock<IUserStore<ApplicationUser>>();
            var httpContext = new Mock<IHttpContextAccessor>();
            var userManger = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
            var userService = new UserService(userManger.Object, httpContext.Object);

            httpContext.SetupGet(h => h.HttpContext.User)
                .Returns(TestHelper.GetUserClaimPrinciple(""));

            userManger.Setup(u => u.FindByIdAsync(user.Id))
                .ReturnsAsync(user)
                .Verifiable();

            userManger.Setup(u => u.UpdateAsync(user))
                .Verifiable();

            //Act
            await userService.DeleteUser(user.Id);

            //Assert
            userManger.Verify();
        }
    }

    [Fact]
    public async void DeleteUser_UserNotExists()
    {
        using (var dbContext = CreateDbContext())
        {
            //Arrange
            var user = TestHelper.CreateUser();

            var store = new Mock<IUserStore<ApplicationUser>>();
            var httpContext = new Mock<IHttpContextAccessor>();
            var userManger = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
            var userService = new UserService(userManger.Object, httpContext.Object);

            httpContext.SetupGet(h => h.HttpContext.User)
                .Returns(TestHelper.GetUserClaimPrinciple(""));

            userManger.Setup(u => u.FindByIdAsync(user.Id))
                .ReturnsAsync((ApplicationUser)null);

            //Act

            //Assert
            var exception = await Assert.ThrowsAsync<EntityNotFoundException>(
              async () => await userService.DeleteUser(user.Id)
            );
        }
    }


    [Fact]
    public async void ListAllUsers()
    {
        using (var dbContext = CreateDbContext())
        {
            //Arrange
            var user = TestHelper.CreateUser();

            var store = new Mock<IUserStore<ApplicationUser>>();
            var httpContext = new Mock<IHttpContextAccessor>();
            var userManger = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
            var userService = new UserService(userManger.Object, httpContext.Object);

            httpContext.SetupGet(h => h.HttpContext.User)
                .Returns(TestHelper.GetUserClaimPrinciple(""));

            userManger.Setup(u => u.GetUsersInRoleAsync(UserRoles.Default))
                .ReturnsAsync(new List<ApplicationUser>() { user })
                .Verifiable();

            //Act
            var result = await userService.ListAllUsers();

            //Assert
            userManger.Verify();
            Assert.Equal(user.Fullname, result.First().FullName);
        }
    }
}
using CaffWebApp.DAL.Entites;
using System.Security.Claims;

namespace CaffWebApp.Test;

public class TestHelper
{
    public static ApplicationUser CreateUser() =>
        new ApplicationUser()
        {
            Fullname = "Carson Alexander",
            UserName = "carson@email.hu",
            Email = "carson@email.hu",
            EmailConfirmed = true,
            PasswordHash = "AQAAAAEAACcQAAAAEAYhQeew7rP4OrFaPD7hY14miQgE+SY2grNxQ01VBp/7AGxnUtJLFZxVj+KLYk/2Rw==",
            SecurityStamp = "QWHJ3YA4ZZ7PH7QGYAB2IU7PLUCA3LBO",
            ConcurrencyStamp = "70ceb6e6-9a79-4fb8-b325-93453e2021b1",
            IsDeleted = false,
        };

    public static Caff CreateCaff() =>
        new Caff()
        {
            CreatorName = "Carson Alexander",
            OriginalFileName = "TestCaffFile",
            StoredFileName = "475c5j45-049c-4d7b-a222-02ebdc22b22r",
            CreatedAt = DateTimeOffset.Now,
            UploadedBy = CreateUser(),
            UploadedAt = DateTimeOffset.Now,
            CiffImages = new List<Ciff>() { CreateCiff() },
            Comments = new List<Comment>() { CreateComment() },
        };


    public static Ciff CreateCiff() =>
        new Ciff()
        {
            Caption = "TestCaption",
            Width = 2000,
            Height = 1000,
            Tags = "Test1,Test2,Test3",
            Duration = 1000,
        };

    public static Comment CreateComment() =>
        new Comment()
        {
            Text = "TestComment",
            CreateAt= DateTimeOffset.Now,
            CreatedBy = CreateUser(),
        };

    public static ClaimsPrincipal GetUserClaimPrinciple(string userId) =>
        new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>() { new Claim(ClaimTypes.NameIdentifier, userId) }));
}

using CaffWebApp.BLL.Dtos.Caff;
using CaffWebApp.BLL.Dtos.Parser;
using CaffWebApp.BLL.Exceptions;
using CaffWebApp.BLL.Services.Caff;
using CaffWebApp.BLL.Services.Parser;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Net.Mime;

namespace CaffWebApp.Test.ServiceUnitTests;

public class CaffServiceTest : SqliteInMemoryDb
{
    [Fact]
    public async void ListCaffImages_All()
    {
        using (var dbContext = CreateDbContext())
        {
            //Arrange
            var sampleCaff1 = TestHelper.CreateCaff();
            var sampleCaff2 = TestHelper.CreateCaff();
            sampleCaff2.CreatorName = "Mark";
            dbContext.CaffImages.AddRange(sampleCaff1, sampleCaff2);
            await dbContext.SaveChangesAsync();

            var parserService = new Mock<IParserService>();
            var httpContext = new Mock<IHttpContextAccessor>();
            var caffService = new CaffService(dbContext, parserService.Object, httpContext.Object);
            var filter = new CaffFilterDto();

            //Act
            var result = await caffService.ListCaffImages(filter);

            //Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(sampleCaff1.CreatorName, result[0].CreatorName);
            Assert.Equal(sampleCaff1.OriginalFileName, result[0].FileName);
            Assert.Equal(sampleCaff1.StoredFileName + ".gif", result[0].FileUri);
            Assert.Equal(sampleCaff1.UploadedAt, result[0].UploadedAt);
            Assert.Equal(sampleCaff1.UploadedBy.Fullname, result[0].UploadedBy);
            Assert.Equal(sampleCaff1.CiffImages.Select(ciff => ciff.Caption).ToHashSet(), result[0].Captions);
            Assert.Equal(sampleCaff1.CiffImages.SelectMany(ciff => ciff.Tags.Split(',')).ToHashSet(), result[0].Tags);
            Assert.Equal(sampleCaff2.CreatorName, result[1].CreatorName);
        }
    }


    [Fact]
    public async void ListCaffImages_Filter()
    {
        using (var dbContext = CreateDbContext())
        {
            //Arrange
            var sampleCaff1 = TestHelper.CreateCaff();
            var sampleCaff2 = TestHelper.CreateCaff();
            sampleCaff2.CreatorName = "Mark";
            dbContext.CaffImages.AddRange(sampleCaff1, sampleCaff2);
            await dbContext.SaveChangesAsync();

            var parserService = new Mock<IParserService>();
            var httpContext = new Mock<IHttpContextAccessor>();
            var caffService = new CaffService(dbContext, parserService.Object, httpContext.Object);
            var filter = new CaffFilterDto()
            {
                SearchText = "Mark"
            };

            //Act
            var result = await caffService.ListCaffImages(filter);

            //Assert
            Assert.Single(result);
            Assert.Equal(sampleCaff2.CreatorName, result[0].CreatorName);
        }
    }

    [Fact]
    public async void GetCaffDetails_Existing()
    {
        using (var dbContext = CreateDbContext())
        {
            //Arrange
            var sampleCaff = TestHelper.CreateCaff();
            dbContext.CaffImages.AddRange(sampleCaff);
            await dbContext.SaveChangesAsync();

            var parserService = new Mock<IParserService>();
            var httpContext = new Mock<IHttpContextAccessor>();
            var caffService = new CaffService(dbContext, parserService.Object, httpContext.Object);

            //Act
            var result = await caffService.GetCaffDetails(sampleCaff.Id);

            //Assert
            Assert.Equal(sampleCaff.CreatorName, result.CreatorName);
            Assert.Equal(sampleCaff.OriginalFileName, result.FileName);
            Assert.Equal(sampleCaff.StoredFileName + ".gif", result.FileUri);
            Assert.Equal(sampleCaff.CreatedAt, result.CreatedAt);
            Assert.Equal(sampleCaff.UploadedAt, result.UploadedAt);
            Assert.Equal(sampleCaff.UploadedBy.Email, result.UploadedBy.Email);
            Assert.Equal(sampleCaff.CiffImages.First().Caption, result.CiffImages.First().Caption);
            Assert.Equal(sampleCaff.Comments.First().Text, result.Comments.First().Text);
        }
    }

    [Fact]
    public async void GetCaffDetails_NotExisting()
    {
        using (var dbContext = CreateDbContext())
        {
            //Arrange
            var parserService = new Mock<IParserService>();
            var httpContext = new Mock<IHttpContextAccessor>();
            var caffService = new CaffService(dbContext, parserService.Object, httpContext.Object);

            //Act

            //Assert
            var exception = await Assert.ThrowsAsync<EntityNotFoundException>(
                async () => await caffService.GetCaffDetails(-1)
            );
        }
    }


    [Fact]
    public async void DeleteCaff_Existing()
    {
        using (var dbContext = CreateDbContext())
        {
            //Arrange
            var sampleCaff = TestHelper.CreateCaff();
            dbContext.CaffImages.AddRange(sampleCaff);
            await dbContext.SaveChangesAsync();

            var parserService = new Mock<IParserService>();
            var httpContext = new Mock<IHttpContextAccessor>();
            var caffService = new CaffService(dbContext, parserService.Object, httpContext.Object);

            //Act
            await caffService.DeleteCaff(sampleCaff.Id);

            //Assert
            var result = await dbContext.CaffImages.SingleOrDefaultAsync(caff => caff.Id == sampleCaff.Id);
            Assert.Null(result);
        }
    }

    [Fact]
    public async void DeleteCaff_NotExisting()
    {
        using (var dbContext = CreateDbContext())
        {
            //Arrange
            var sampleCaff = TestHelper.CreateCaff();
            dbContext.CaffImages.AddRange(sampleCaff);
            await dbContext.SaveChangesAsync();

            var parserService = new Mock<IParserService>();
            var httpContext = new Mock<IHttpContextAccessor>();
            var caffService = new CaffService(dbContext, parserService.Object, httpContext.Object);

            //Act

            //Assert
            var exception = await Assert.ThrowsAsync<EntityNotFoundException>(
               async () => await caffService.DeleteCaff(-1)
           );
        }
    }

    [Fact]
    public async void DownloadCaff_Existing()
    {
        using (var dbContext = CreateDbContext())
        {
            //Arrange
            var sampleCaff = TestHelper.CreateCaff();
            dbContext.CaffImages.AddRange(sampleCaff);
            await dbContext.SaveChangesAsync();

            var parserService = new Mock<IParserService>();
            var httpContext = new Mock<IHttpContextAccessor>();
            var caffService = new CaffService(dbContext, parserService.Object, httpContext.Object);

            parserService.Setup(p => p.GetCaffFileContent(sampleCaff.StoredFileName))
                .ReturnsAsync(new byte[] { 0 });

            //Act
            var result = await caffService.DownloadCaff(sampleCaff.Id);

            //Assert
            Assert.Equal(sampleCaff.OriginalFileName, result.FileName);
            Assert.Equal(new byte[] { 0 }, result.Content);
            Assert.Equal(MediaTypeNames.Application.Octet, result.MimeType);
        }
    }

    [Fact]
    public async void DownloadCaff_NotExisting()
    {
        using (var dbContext = CreateDbContext())
        {
            //Arrange
            var parserService = new Mock<IParserService>();
            var httpContext = new Mock<IHttpContextAccessor>();
            var caffService = new CaffService(dbContext, parserService.Object, httpContext.Object);

            //Act

            //Assert
            var exception = await Assert.ThrowsAsync<EntityNotFoundException>(
              async () => await caffService.DownloadCaff(-1)
            );
        }
    }

    [Fact]
    public async void UploadCaff()
    {
        using (var dbContext = CreateDbContext())
        {
            //Arrange
            var user = TestHelper.CreateUser();
            dbContext.Add(user);
            await dbContext.SaveChangesAsync();
            var parsedCaff = new CaffParsedDto()
            {
                StoredFileName = "StoredName",
                OriginalFileName = "OriginalName",
                CreatorName = "CreatorName",
                CreatedAt = DateTimeOffset.Now,
                CiffData = new List<CiffParsedDto>() {
                    new CiffParsedDto() {
                        Caption = "TestCaption",
                        Width = 2000,
                        Height = 1000,
                        Tags = "Test1,Test2,Test3",
                        Duration = 1000,
                    },
                }
            };

            var parserService = new Mock<IParserService>();
            var httpContext = new Mock<IHttpContextAccessor>();
            var caffService = new CaffService(dbContext, parserService.Object, httpContext.Object);

            parserService.Setup(p => p.ParseCaffFile(It.IsAny<AddCaffDto>()))
                .ReturnsAsync(parsedCaff);

            httpContext.SetupGet(h => h.HttpContext!.User)
                .Returns(TestHelper.GetUserClaimPrinciple(user.Id));

            //Act
            var result = await caffService.UploadCaffFile(new AddCaffDto());

            //Assert
            Assert.Equal(parsedCaff.OriginalFileName, result.FileName);
            Assert.Equal(parsedCaff.StoredFileName + ".gif", result.FileUri);
            Assert.Equal(parsedCaff.CreatorName, result.CreatorName);
            Assert.Equal(parsedCaff.CreatedAt, result.CreatedAt);
            Assert.Equal(user.Email, result.UploadedBy.Email);
            Assert.Equal(parsedCaff.CiffData.First().Caption, result.CiffImages.First().Caption);
            Assert.Equal(parsedCaff.CiffData.First().Width, result.CiffImages.First().Width);
            Assert.Equal(parsedCaff.CiffData.First().Height, result.CiffImages.First().Height);
            Assert.Equal(parsedCaff.CiffData.First().Duration, result.CiffImages.First().Duration);
            Assert.Equal(parsedCaff.CiffData.First().Tags, string.Join(',', result.CiffImages.First().Tags));
        }
    }
}
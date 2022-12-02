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
            Assert.Equal(sampleCaff1.StoredFileName, result[0].FileName);
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
            Assert.Equal(sampleCaff.StoredFileName, result.FileName);
            Assert.Equal(sampleCaff.AnimationDuration, result.AnimationDuration);
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
            var sampleCaff = TestHelper.CreateCaff();
            dbContext.CaffImages.AddRange(sampleCaff);
            await dbContext.SaveChangesAsync();

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
            Assert.Equal(result.FileName, sampleCaff.OriginalFileName);
            Assert.Equal(result.Content, new byte[] { 0 });
            Assert.Equal(MediaTypeNames.Application.Octet, result.MimeType);
        }
    }

    [Fact]
    public async void DownloadCaff_NotExisting()
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

            //Assert
            var exception = await Assert.ThrowsAsync<EntityNotFoundException>(
              async () => await caffService.DownloadCaff(-1)
            );
        }
    }

    [Fact]
    public async void UploadCaff_Existing()
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
                CreaterName = "CreatorName",
                CreatedAt = DateTimeOffset.Now,
                AnimationDuration = 10,
                CiffData = new List<CiffParsedDto>() {
                    new CiffParsedDto() {
                        Caption = "TestCaption",
                        Width = 2000,
                        Height = 1000,
                        Tags = "Test1,Test2,Test3"
                    },
                }
            };

            var parserService = new Mock<IParserService>();
            var httpContext = new Mock<IHttpContextAccessor>();
            var caffService = new CaffService(dbContext, parserService.Object, httpContext.Object);

            parserService.Setup(p => p.ParseCaffFile(It.IsAny<AddCaffDto>()))
                .ReturnsAsync(parsedCaff);

            httpContext.SetupGet(h => h.HttpContext.User)
                .Returns(TestHelper.GetUserClaimPrinciple(user.Id));

            //Act
            var result = await caffService.UploadCaffFile(new AddCaffDto());

            //Assert
            Assert.Equal(result.FileName, parsedCaff.StoredFileName);
            Assert.Equal(result.CreatorName, parsedCaff.CreaterName);
            Assert.Equal(result.CreatedAt, parsedCaff.CreatedAt);
            Assert.Equal(result.AnimationDuration, parsedCaff.AnimationDuration);
            Assert.Equal(result.UploadedBy.Email, user.Email);
            Assert.Equal(result.CiffImages.First().Caption, parsedCaff.CiffData.First().Caption);
            Assert.Equal(result.CiffImages.First().Width, parsedCaff.CiffData.First().Width);
            Assert.Equal(result.CiffImages.First().Height, parsedCaff.CiffData.First().Height);
            Assert.Equal(string.Join(',', result.CiffImages.First().Tags), parsedCaff.CiffData.First().Tags);
        }
    }
}
using CaffWebApp.BLL.Dtos.Caff;
using IdentityModel.Client;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace CaffWebApp.Test.ApiIntegrationTests;

public class CaffControllerTests : WebServerFixture
{
    [Fact]
    public async Task ListCaffImages_All()
    {
        //Arrange
        var sampleCaff1 = TestHelper.CreateCaff();
        var sampleCaff2 = TestHelper.CreateCaff();
        sampleCaff2.CreatorName = "Mark";
        DbContext.CaffImages.AddRange(sampleCaff1, sampleCaff2);
        await DbContext.SaveChangesAsync();

        var filter = new CaffFilterDto()
        {
            SearchText = ""
        };

        var accessToken = await apiServer.GetAccessToken("admin@email.hu", "Test.54321");
        var client = apiServer.CreateClient();
        client.SetBearerToken(accessToken);

        //Act
        var caffListResponse = await client.GetAsync("/api/caff?" + filter.ToQueryParams());
        var result = await caffListResponse.Content.ReadFromJsonAsync<List<CaffDto>>();

        //Assert
        Assert.Equal(HttpStatusCode.OK, caffListResponse.StatusCode);
        Assert.Equal(2, result!.Count);
        Assert.Equal(sampleCaff1.CreatorName, result[0].CreatorName);
        Assert.Equal(sampleCaff1.StoredFileName, result[0].FileName);
        Assert.Equal(sampleCaff1.UploadedAt, result[0].UploadedAt);
        Assert.Equal(sampleCaff1.UploadedBy.Fullname, result[0].UploadedBy);
        Assert.Equal(sampleCaff1.CiffImages.Select(ciff => ciff.Caption).ToHashSet(), result[0].Captions);
        Assert.Equal(sampleCaff1.CiffImages.SelectMany(ciff => ciff.Tags.Split(',')).ToHashSet(), result[0].Tags);
        Assert.Equal(sampleCaff2.CreatorName, result[1].CreatorName);
    }

    [Fact]
    public async Task ListCaffImages_Filter()
    {
        //Arrange
        var sampleCaff1 = TestHelper.CreateCaff();
        var sampleCaff2 = TestHelper.CreateCaff();
        sampleCaff2.CreatorName = "Mark";
        DbContext.CaffImages.AddRange(sampleCaff1, sampleCaff2);
        await DbContext.SaveChangesAsync();

        var filter = new CaffFilterDto()
        {
            SearchText = "Mark"
        };

        var accessToken = await apiServer.GetAccessToken("admin@email.hu", "Test.54321");
        var client = apiServer.CreateClient();
        client.SetBearerToken(accessToken);

        //Act
        var caffListResponse = await client.GetAsync("/api/caff?" + filter.ToQueryParams());
        var result = await caffListResponse.Content.ReadFromJsonAsync<List<CaffDto>>();

        //Assert
        Assert.Equal(HttpStatusCode.OK, caffListResponse.StatusCode);
        Assert.Single(result!);
        Assert.Equal(sampleCaff2.CreatorName, result[0]!.CreatorName);
    }

    [Fact]
    public async Task GetCaffDetails_Existing()
    {
        //Arrange
        var sampleCaff = TestHelper.CreateCaff();
        DbContext.CaffImages.AddRange(sampleCaff);
        await DbContext.SaveChangesAsync();

        var accessToken = await apiServer.GetAccessToken("admin@email.hu", "Test.54321");
        var client = apiServer.CreateClient();
        client.SetBearerToken(accessToken);

        //Act
        var caffDetailsResponse = await client.GetAsync("/api/caff/" + sampleCaff.Id);
        var result = await caffDetailsResponse.Content.ReadFromJsonAsync<CaffDetailsDto>();

        //Assert
        Assert.Equal(HttpStatusCode.OK, caffDetailsResponse.StatusCode);
        Assert.Equal(sampleCaff.CreatorName, result!.CreatorName);
        Assert.Equal(sampleCaff.OriginalFileName, result.FileName);
        Assert.Equal(sampleCaff.StoredFileName + ".gif", result.FileUri);
        Assert.Equal(sampleCaff.CreatedAt, result.CreatedAt);
        Assert.Equal(sampleCaff.UploadedAt, result.UploadedAt);
        Assert.Equal(sampleCaff.UploadedBy.Email, result.UploadedBy.Email);
        Assert.Equal(sampleCaff.CiffImages.First().Caption, result.CiffImages.First().Caption);
        Assert.Equal(sampleCaff.Comments.First().Text, result.Comments.First().Text);
    }

    [Fact]
    public async Task GetCaffDetails_NotExisting()
    {
        //Arrange
        var accessToken = await apiServer.GetAccessToken("admin@email.hu", "Test.54321");
        var client = apiServer.CreateClient();
        client.SetBearerToken(accessToken);

        //Act
        var caffDetailsResponse = await client.GetAsync("/api/caff/" + 0);

        //Assert
        Assert.Equal(HttpStatusCode.NotFound, caffDetailsResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteCaff_Existing()
    {
        //Arrange
        var sampleCaff = TestHelper.CreateCaff();
        DbContext.CaffImages.AddRange(sampleCaff);
        await DbContext.SaveChangesAsync();

        var accessToken = await apiServer.GetAccessToken("admin@email.hu", "Test.54321");
        var client = apiServer.CreateClient();
        client.SetBearerToken(accessToken);

        //Act
        var caffDeleteResponse = await client.DeleteAsync("/api/caff/" + sampleCaff.Id);

        //Assert
        Assert.Equal(HttpStatusCode.OK, caffDeleteResponse.StatusCode);
        var result = await DbContext.CaffImages.SingleOrDefaultAsync(caff => caff.Id == sampleCaff.Id);
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteCaff_NotExisting()
    {
        //Arrange
        var accessToken = await apiServer.GetAccessToken("admin@email.hu", "Test.54321");
        var client = apiServer.CreateClient();
        client.SetBearerToken(accessToken);

        //Act
        var caffDeleteResponse = await client.DeleteAsync("/api/caff/" + 0);

        //Assert
        Assert.Equal(HttpStatusCode.NotFound, caffDeleteResponse.StatusCode);
    }

    [Fact]
    public async Task DownloadCaff_Existing()
    {
        //Arrange
        var accessToken = await apiServer.GetAccessToken("admin@email.hu", "Test.54321");
        var client = apiServer.CreateClient();
        client.SetBearerToken(accessToken);

        HttpResponseMessage imageUploadResponse;
        using (var imageFile = File.OpenRead("ApiIntegrationTests/caff/1.caff"))
        using (var content = new StreamContent(imageFile))
        using (var formData = new MultipartFormDataContent())
        {
            formData.Add(content, "caffFile", "1.caff");
            imageUploadResponse = await client.PostAsync("api/caff/upload", formData);
        }
        var caffDetails = await imageUploadResponse.Content.ReadFromJsonAsync<CaffDetailsDto>();

        //Act
        var caffDownloadResponse = await client.GetAsync($"api/caff/{caffDetails!.Id}/download");

        byte[] fileBytes;
        using (var stream = new MemoryStream())
        {
            await caffDownloadResponse.Content.CopyToAsync(stream);
            fileBytes = stream.ToArray();
        }

        byte[] actualFileBytes;
        using (var imageFile = File.OpenRead("ApiIntegrationTests/caff/1.caff"))
        using (var stream = new MemoryStream())
        {
            await imageFile.CopyToAsync(stream);
            actualFileBytes = stream.ToArray();
        }

        //Assert
        Assert.Equal(HttpStatusCode.OK, caffDownloadResponse.StatusCode);
        Assert.NotEmpty(fileBytes);
        Assert.Equal(actualFileBytes, fileBytes);
    }


    [Fact]
    public async Task DownloadCaff_NotExisting()
    {
        //Arrange
        var accessToken = await apiServer.GetAccessToken("admin@email.hu", "Test.54321");
        var client = apiServer.CreateClient();
        client.SetBearerToken(accessToken);

        //Act
        var caffDownloadResponse = await client.GetAsync("/api/caff/0/download");

        //Assert
        Assert.Equal(HttpStatusCode.NotFound, caffDownloadResponse.StatusCode);
    }

    [Fact]
    public async Task UploadCaff()
    {
        //Arrange
        var sampleCaff = TestHelper.CreateCaff();
        DbContext.CaffImages.AddRange(sampleCaff);
        await DbContext.SaveChangesAsync();

        var accessToken = await apiServer.GetAccessToken("admin@email.hu", "Test.54321");
        var client = apiServer.CreateClient();
        client.SetBearerToken(accessToken);

        //Act
        HttpResponseMessage imageUploadResponse;
        using (var imageFile = File.OpenRead("ApiIntegrationTests/caff/1.caff"))
        using (var content = new StreamContent(imageFile))
        using (var formData = new MultipartFormDataContent())
        {
            formData.Add(content, "caffFile", "1.caff");
            imageUploadResponse = await client.PostAsync("api/caff/upload", formData);
        }

        var result = await imageUploadResponse.Content.ReadFromJsonAsync<CaffDetailsDto>();
        var imageAvailableResponse = await client.GetAsync($"{Configuration.GetSection("WebServer:WebAPIUrl").Value}/images/{result!.FileUri}");

        //Assert
        Assert.Equal(HttpStatusCode.OK, imageUploadResponse.StatusCode);
        Assert.Equal("Test Creator", result.CreatorName);
        Assert.Equal("1.caff", result.FileName);
        Assert.Equal("Beautiful scenery", result.CiffImages.First().Caption);
        Assert.Equal(1000, result.CiffImages.First().Width);
        Assert.Equal(667, result.CiffImages.First().Height);
        Assert.Equal(1000, sampleCaff.CiffImages.First().Duration);
        Assert.Equal("landscape,sunset,mountains", string.Join(',', result.CiffImages.First().Tags));
        Assert.Equal(HttpStatusCode.OK, imageAvailableResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteCaff_NormalUser()
    {
        //Arrange
        var user = TestHelper.CreateUser();
        var sampleCaff = TestHelper.CreateCaff();
        DbContext.Add(user);
        DbContext.Add(sampleCaff);
        await DbContext.SaveChangesAsync();

        var accessToken = await apiServer.GetAccessToken(user.Email, "Test.54321");
        var client = apiServer.CreateClient();
        client.SetBearerToken(accessToken);

        //Act
        var caffDeleteResponse = await client.DeleteAsync("/api/caff/" + sampleCaff.Id);

        //Assert
        Assert.Equal(HttpStatusCode.Unauthorized, caffDeleteResponse.StatusCode);
    }

    [Fact]
    public async Task ListCaffImages_NotRegisteredUser()
    {
        //Arrange
        var client = apiServer.CreateClient();

        //Act
        var caffDeleteResponse = await client.GetAsync("/api/caff");

        //Assert
        Assert.Equal(HttpStatusCode.Unauthorized, caffDeleteResponse.StatusCode);
    }
}

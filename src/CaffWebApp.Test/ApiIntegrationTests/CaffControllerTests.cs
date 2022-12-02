using CaffWebApp.BLL.Dtos.Caff;
using IdentityModel.Client;
using Microsoft.EntityFrameworkCore;
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
        var a = await DbContext.CaffImages.ToListAsync();
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

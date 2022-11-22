using CaffWebApp.BLL.Dtos.Caff;

namespace CaffWebApp.BLL.Services.Caff;

public class CaffService : ICaffService
{
    public Task DeleteCaff(int caddId)
    {
        throw new NotImplementedException();
    }

    public Task<FileResultDto> DownloadCaff(int caffId)
    {
        throw new NotImplementedException();
    }

    public Task<CaffDetailsDto> GetCaffDetails(int caffId)
    {
        throw new NotImplementedException();
    }

    public Task<List<CaffDto>> ListCaffImages(CaffFilterDto filter)
    {
        throw new NotImplementedException();
    }

    public Task<CaffDetailsDto> UploadCaffFile(AddCaffDto caffDto)
    {
        throw new NotImplementedException();
    }
}

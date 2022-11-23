using CaffWebApp.BLL.Dtos.Caff;
using CaffWebApp.BLL.Dtos.Parser;
using Microsoft.Extensions.Hosting;

namespace CaffWebApp.BLL.Services.Parser;

public class ParserService : IParserService
{
    private readonly IHostEnvironment _environment;
    private const string BaseCaffFolder = "wwwroot/caff";

    public ParserService(IHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<byte[]> GetCaffFileContent(string fileName)
    {
        return await File.ReadAllBytesAsync(Path.Combine(_environment.ContentRootPath, BaseCaffFolder, fileName));
    }

    public Task<CaffParsedDto> ParseCaffFile(AddCaffDto caffDto)
    {
        throw new NotImplementedException();
    }
}

using CaffWebApp.BLL.Dtos.Caff;
using CaffWebApp.BLL.Dtos.Parser;
using CaffWebApp.BLL.Options;
using Microsoft.Extensions.Options;

namespace CaffWebApp.BLL.Services.Parser;

public class ParserService : IParserService
{
    private readonly CaffImagePathOptions imagePath;

    public ParserService(IOptions<CaffImagePathOptions> imagePath)
    {
        this.imagePath = imagePath.Value;
    }

    public async Task<byte[]> GetCaffFileContent(string fileName)
    {
        return await File.ReadAllBytesAsync(Path.Combine(imagePath.Raw, fileName));
    }

    public Task<CaffParsedDto> ParseCaffFile(AddCaffDto caffDto)
    {
        throw new NotImplementedException();
    }
}

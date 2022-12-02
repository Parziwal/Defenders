using CaffWebApp.BLL.Dtos.Caff;
using CaffWebApp.BLL.Dtos.Parser;
using CaffWebApp.BLL.Exceptions;
using CaffWebApp.BLL.Options;
using Microsoft.Extensions.Options;
using System.Runtime.InteropServices;

namespace CaffWebApp.BLL.Services.Parser;

public class ParserService : IParserService
{
    private readonly CaffImagePathOptions _imagePath;

    [DllImport("native/CaffWebApp.Parser.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int ParseCaffFile(string filePath, string outputPath);

    public ParserService(IOptions<CaffImagePathOptions> imagePath)
    {
        this._imagePath = imagePath.Value;
    }

    public async Task<byte[]> GetCaffFileContent(string fileName)
    {
        return await File.ReadAllBytesAsync(Path.Combine(_imagePath.Raw, fileName + ".caff"));
    }

    public async Task<CaffParsedDto> ParseCaffFile(AddCaffDto caffDto)
    {
        var parsedCaff = new CaffParsedDto()
        {
            StoredFileName = Guid.NewGuid().ToString(),
            OriginalFileName = caffDto.CaffFile.FileName,
            CreatorName = "Test",
            CreatedAt = DateTimeOffset.Now,
            CiffData = new()
        };
        
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), _imagePath.Raw, parsedCaff.StoredFileName + ".caff");
        string outputPath = Path.Combine(Directory.GetCurrentDirectory(), _imagePath.Parsed, parsedCaff.StoredFileName + ".gif");
        using (Stream fileStream = new FileStream(filePath, FileMode.Create))
        {
            await caffDto.CaffFile.CopyToAsync(fileStream);
        }

        try
        {
            if (ParseCaffFile(filePath, outputPath) != 0)
                throw new Exception();
        }
        catch (Exception)
        {
            throw new ParserException("Parser exception occured!");
        }
        

        using (var metaDataReader = new StreamReader(outputPath + ".metadata"))
        {
            var numAnim = Convert.ToInt32(metaDataReader.ReadLine());
            parsedCaff.CreatedAt = DateTimeOffset.Parse(metaDataReader.ReadLine());
            parsedCaff.CreatorName = metaDataReader.ReadLine() ?? "";
            
            for (int i = 0; !metaDataReader.EndOfStream && i < numAnim; i++)
            {
                var size = metaDataReader.ReadLine().Split('x');
                parsedCaff.CiffData.Add(new());
                parsedCaff.CiffData[i].Width = Convert.ToInt32(size[0]);
                parsedCaff.CiffData[i].Height = Convert.ToInt32(size[1]);
                parsedCaff.CiffData[i].Duration = Convert.ToInt32(metaDataReader.ReadLine());
                parsedCaff.CiffData[i].Caption = metaDataReader.ReadLine();
                parsedCaff.CiffData[i].Tags = metaDataReader.ReadLine();
            }
        }
        File.Delete(outputPath + ".metadata");
        return parsedCaff;
    }
}

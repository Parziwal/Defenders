using CaffWebApp.BLL.Dtos.Caff;
using CaffWebApp.BLL.Exceptions;
using CaffWebApp.BLL.Extensions;
using CaffWebApp.BLL.Services.Parser;
using CaffWebApp.DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Net.Mime;

namespace CaffWebApp.BLL.Services.Caff;

public class CaffService : ICaffService
{
    private readonly CaffDbContext _dbContext;
    private readonly IParserService _parserService;
    private readonly IHttpContextAccessor _httpContext;

    public CaffService(CaffDbContext dbContext, IParserService parserService, IHttpContextAccessor httpContext)
    {
        _dbContext = dbContext;
        _parserService = parserService;
        _httpContext = httpContext;
    }

    public async Task<List<CaffDto>> ListCaffImages(CaffFilterDto filter) =>
        await _dbContext.CaffImages
                .Include(caff => caff.CiffImages)
                .Include(caff => caff.UploadedBy)
                .Where(caff => !string.IsNullOrEmpty(filter.SearchText) && caff.CreatorName.Contains(filter.SearchText))
                .Where(caff => !string.IsNullOrEmpty(filter.SearchText) &&
                    caff.CiffImages.Any(ciff => ciff.Caption.Contains(filter.SearchText) || ciff.Tags.Contains(filter.SearchText)))
                .Select(caff => new CaffDto(caff))
                .ToListAsync();

    public async Task<CaffDetailsDto> GetCaffDetails(int caffId)
    {
        var caff = await _dbContext.CaffImages
                .Include(caff => caff.CiffImages)
                .Include(caff => caff.UploadedBy)
                .Include(caff => caff.Comments)
                    .ThenInclude(comment => comment.CreatedBy)
                .SingleOrDefaultAsync(caff => caff.Id == caffId);

        if (caff == null)
        {
            throw new EntityNotFoundException($"Caff with {caffId} id does not exists!");
        }

        return new CaffDetailsDto(caff);
    }

    public async Task DeleteCaff(int caffId)
    {
        var caff = await _dbContext.CaffImages
                        .Include(caff => caff.CiffImages)
                        .Include(caff => caff.Comments)
                        .SingleOrDefaultAsync(caff => caff.Id == caffId);

        if (caff == null)
        {
            throw new EntityNotFoundException($"Caff with {caffId} id does not exists!");
        }

        _dbContext.Remove(caff);

        await _dbContext.SaveChangesAsync();
    }

    public async Task<FileResultDto> DownloadCaff(int caffId)
    {
        var caff = await _dbContext.CaffImages
                        .SingleOrDefaultAsync(caff => caff.Id == caffId);

        if (caff == null)
        {
            throw new EntityNotFoundException($"Caff with {caffId} id does not exists!");
        }

        var content = await _parserService.GetCaffFileContent(caff.StoredFileName);
        return new FileResultDto(content, caff.OriginalFileName, MediaTypeNames.Application.Octet);
    }

    public async Task<CaffDetailsDto> UploadCaffFile(AddCaffDto caffDto)
    {
        var parsedCaff = await _parserService.ParseCaffFile(caffDto);

        var caff = new DAL.Entites.Caff()
        {
            CreatorName = parsedCaff.CreaterName,
            AnimationDuration = parsedCaff.AnimationDuration,
            OriginalFileName = parsedCaff.FileName,
            StoredFileName = Guid.NewGuid().ToString(),
            CreatedAt = parsedCaff.CreatedAt,
            UploadedById = _httpContext.GetCurrentUserId(),
            UploadedAt = DateTimeOffset.Now,
            CiffImages = parsedCaff.CiffData.Select(ciff =>
                new DAL.Entites.Ciff()
                {
                    Caption = ciff.Caption,
                    Tags = ciff.Tags,
                    Width = ciff.Width,
                    Height = ciff.Height
                })
                .ToList(),
        };

        _dbContext.Add(caff);
        await _dbContext.SaveChangesAsync();

        return new CaffDetailsDto(caff);
    }
}

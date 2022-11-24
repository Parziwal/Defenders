﻿using CaffWebApp.BLL.Dtos.Caff;
using CaffWebApp.BLL.Services.Caff;
using Microsoft.AspNetCore.Mvc;

namespace CaffWebApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CaffController : ControllerBase
    {
        private readonly ICaffService _caffService;

        public CaffController(ICaffService caffService)
        {
            _caffService = caffService;
        }

        [HttpGet]
        public Task<List<CaffDto>> ListCaffImages([FromQuery] CaffFilterDto filter) =>
            _caffService.ListCaffImages(filter);


        [HttpGet("{caffId}")]
        public Task<CaffDetailsDto> GetCaffDetails(int caffId) =>
            _caffService.GetCaffDetails(caffId);

        [HttpGet("{caffId}/download")]
        public async Task<FileResult> DownloadCaffFile(int caffId)
        {
            var caffFile = await _caffService.DownloadCaff(caffId);
            return File(caffFile.Content, caffFile.MimeType, caffFile.FileName);
        }

        [HttpPost("upload")]
        public Task<CaffDetailsDto> UploadCaffFile([FromForm] AddCaffDto caffDto) =>
            _caffService.UploadCaffFile(caffDto);

        [HttpDelete("{caffId}")]
        public Task DeleteCaff(int caffId) =>
            _caffService.DeleteCaff(caffId);
    }
}
//using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceHive360.Admin.Application.Services.Files
{
    public interface IFileService
    {
        Task<List<string>> SaveImagesAsync(List<IFormFile> files, string subFolder);
        void DeleteImages(List<string> imageUrls);
    }
}

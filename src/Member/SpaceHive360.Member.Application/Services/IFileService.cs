using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpaceHive360.Member.Application.Services
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile file, string folder);
        Task<List<string>> SaveFilesAsync(List<IFormFile> files, string folder);
        void DeleteFile(string filePath);
    }
}

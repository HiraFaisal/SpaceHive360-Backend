using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using SpaceHive360.Admin.Application.Services.Files;
using Microsoft.Extensions.Hosting;

namespace SpaceHive360.Admin.Infrastructure.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _env;

        public FileService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<List<string>> SaveImagesAsync(List<IFormFile> files, string subFolder)
        {
            var savedUrls = new List<string>();

            // e.g. wwwroot/uploads/plans
            var uploadPath = Path.Combine(_env.WebRootPath, "uploads", subFolder);

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            foreach (var file in files)
            {
                if (file.Length == 0) continue;

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(ext))
                    throw new InvalidOperationException($"File type '{ext}' is not allowed.");

                var fileName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Return relative URL for storage and serving
                savedUrls.Add($"/uploads/{subFolder}/{fileName}");
            }

            return savedUrls;
        }

        public void DeleteImages(List<string> imageUrls)
        {
            foreach (var url in imageUrls)
            {
                // Convert URL path to physical path
                var relativePath = url.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
                var fullPath = Path.Combine(_env.WebRootPath, relativePath);

                if (File.Exists(fullPath))
                    File.Delete(fullPath);
            }
        }
    }
}
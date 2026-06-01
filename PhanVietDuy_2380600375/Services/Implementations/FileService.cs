using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using PhanVietDuy_2380600375.Services.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PhanVietDuy_2380600375.Services.Implementations
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _env;

        public FileService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<(string Url, string FilePath)> SaveProductImageAsync(IFormFile file, string slug, string suffix)
        {
            return await ProcessAndSaveImageAsync(file, "products", $"{slug}_{suffix}", 900, 675);
        }

        public async Task<(string Url, string FilePath)> SaveAvatarAsync(IFormFile file, string userId)
        {
            return await ProcessAndSaveImageAsync(file, "avatars", userId, 400, 400);
        }

        public async Task<string> SaveTempAsync(IFormFile file)
        {
            var folderPath = Path.Combine(_env.WebRootPath, "uploads", "temp");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var fileName = $"{Guid.NewGuid():N}_{file.FileName}";
            var filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/uploads/temp/{fileName}";
        }

        public void DeleteFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return;
            
            var fullPath = Path.Combine(_env.WebRootPath, filePath.TrimStart('/'));
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }

        private async Task<(string Url, string FilePath)> ProcessAndSaveImageAsync(IFormFile file, string folderName, string fileNameBase, int width, int height)
        {
            var folderPath = Path.Combine(_env.WebRootPath, "uploads", folderName);
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var fileName = $"{fileNameBase}_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid().ToString("N").Substring(0, 8)}.webp";
            var filePath = Path.Combine(folderPath, fileName);

            using (var image = await Image.LoadAsync(file.OpenReadStream()))
            {
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(width, height),
                    Mode = ResizeMode.Crop
                }));

                await image.SaveAsWebpAsync(filePath);
            }

            return ($"/uploads/{folderName}/{fileName}", $"/uploads/{folderName}/{fileName}");
        }
    }
}

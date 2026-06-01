using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace PhanVietDuy_2380600375.Services.Interfaces
{
    public interface IFileService
    {
        Task<(string Url, string FilePath)> SaveProductImageAsync(IFormFile file, string slug, string suffix);
        Task<(string Url, string FilePath)> SaveAvatarAsync(IFormFile file, string userId);
        Task<string> SaveTempAsync(IFormFile file);
        void DeleteFile(string filePath);
    }
}

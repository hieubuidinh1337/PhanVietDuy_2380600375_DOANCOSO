using System.Threading.Tasks;

namespace PhanVietDuy_2380600375.Services.Interfaces
{
    public interface IApiLogService
    {
        Task LogAsync(string endpoint, string method, int statusCode, string? body, string? userId, string? ip, int durationMs);
    }
}

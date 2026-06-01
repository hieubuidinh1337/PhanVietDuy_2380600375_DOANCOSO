using System.Collections.Generic;

namespace PhanVietDuy_2380600375.Models.DTOs
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }
        public int StatusCode { get; set; }

        public static ApiResponse<T> Ok(T data, string? msg = null)
            => new() { Success = true, Data = data, Message = msg, StatusCode = 200 };

        public static ApiResponse<T> Fail(string msg, int code = 400)
            => new() { Success = false, Message = msg, StatusCode = code };
    }
}

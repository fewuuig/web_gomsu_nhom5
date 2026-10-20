using System;

namespace QuanLyBanDoGomSu.Exception
{
    /// <summary>
    /// Wrapper chuẩn duy nhất cho tất cả các Response trả về FE (Cả Success lẫn Exception)
    /// </summary>
    /// <typeparam name="T">Kiểu dữ liệu Payload trong trường Data</typeparam>
    public class ApiResponse<T>
    {
        public int Status { get; set; }           // HTTP Status Code (200, 400, 404, 500...)
        public string? Message { get; set; }       // Thông báo kết quả
        public T? Data { get; set; }               // Payload dữ liệu (null nếu lỗi)
        public ApiErrorDetail? Error { get; set; } // Chi tiết lỗi (null nếu thành công)
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public static ApiResponse<T> SuccessResponse(T data, string message = "Thành công")
        {
            return new ApiResponse<T>
            {
                Status = 200,
                Message = message,
                Data = data,
                Error = null
            };
        }

        public static ApiResponse<T> ErrorResponse(int status, string message, string? errorDetails = null)
        {
            return new ApiResponse<T>
            {
                Status = status,
                Message = message,
                Data = default,
                Error = new ApiErrorDetail { Code = status, Details = errorDetails ?? message }
            };
        }
    }

    public class ApiErrorDetail
    {
        public int Code { get; set; }
        public string? Details { get; set; }
    }
}

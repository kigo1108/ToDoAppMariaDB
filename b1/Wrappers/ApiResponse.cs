namespace b1.Wrappers
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public List<string> Error { get; set; }

        public static ApiResponse<T>SuccessResponse (T data, string? message = "Thành công")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data,
            };
        }
        public static ApiResponse<T> ErrorResponse(List<string> errors, string? message = "Đã xảy ra lỗi")
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Error = errors
            };
        }
    }
}

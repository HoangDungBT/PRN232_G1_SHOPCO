using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHOP.CO.Application.Common
{
    public class ResultModel<T>
    {
        public bool IsSuccess { get; set; }
        public int Code { get; set; } // Map với HTTP Status Code (200, 400, 404, 500)
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }

        // 1. Dùng khi thành công
        public static ResultModel<T> Success(T? data, string message = "Thành công", int code = 200)
        {
            return new ResultModel<T> { IsSuccess = true, Data = data, Message = message, Code = code };
        }

        // 2. Dùng khi lỗi nghiệp vụ (VD: Sai mật khẩu, Hết hàng)
        public static ResultModel<T> Error(string message, int code = 400)
        {
            return new ResultModel<T> { IsSuccess = false, Message = message, Code = code };
        }

        // 3. Dùng trong khối catch() khi hệ thống gặp lỗi (Sập DB, lỗi code)
        public static ResultModel<T> Exception(Exception ex)
        {
            // Trong thực tế, bạn nên ghi (log) cái ex.Message này ra file, 
            // Còn trả về cho User thì chỉ báo "Lỗi hệ thống" để bảo mật.
            return new ResultModel<T> { IsSuccess = false, Message = $"Lỗi hệ thống: {ex.Message}", Code = 500 };
        }
    }
}

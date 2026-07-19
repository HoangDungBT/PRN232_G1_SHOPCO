using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;

namespace SHOP.CO.Application.Utilities
{
    public static class SlugHelper
    {
        public static string GenerateSlug(string phrase)
        {
            if (string.IsNullOrWhiteSpace(phrase)) return string.Empty;

            string str = phrase.ToLower();
            str = Regex.Replace(str, @"á|à|ả|ạ|ã|ă|ắ|ằ|ẳ|ặ|ẵ|â|ấ|ầ|ẩ|ậ|ẫ", "a");
            str = Regex.Replace(str, @"é|è|ẻ|ẹ|ẽ|ê|ế|ề|ể|ệ|ễ", "e");
            str = Regex.Replace(str, @"í|ì|ỉ|ị|ĩ", "i");
            str = Regex.Replace(str, @"ó|ò|ỏ|ọ|õ|ô|ố|ồ|ổ|ộ|ỗ|ơ|ớ|ờ|ở|ợ|ỡ", "o");
            str = Regex.Replace(str, @"ú|ù|ủ|ụ|ũ|ư|ứ|ừ|ử|ự|ữ", "u");
            str = Regex.Replace(str, @"ý|ỳ|ỷ|ỵ|ỹ", "y");
            str = Regex.Replace(str, @"đ", "d");
            str = Regex.Replace(str, @"[^a-z0-9\s-]", ""); // Xóa ký tự đặc biệt
            str = Regex.Replace(str, @"\s+", " ").Trim(); // Xóa khoảng trắng thừa
            str = Regex.Replace(str, @"\s", "-"); // Thay khoảng trắng bằng dấu -
            return str;
        }
    }
}

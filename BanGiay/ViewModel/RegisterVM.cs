using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BanGiay.ViewModel
{
    public class RegisterVM
    {
        [Required(ErrorMessage = "Không được bỏ trống tên")]
        public string firstName { set; get; }
        [Required(ErrorMessage = "Không được bỏ trống họ")]
        public string lastName { set; get; }
        [Required(ErrorMessage = "Không được bỏ trống email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string email { set; get; }
        [Required(ErrorMessage = "Không được bỏ trống số điện thoại")]
        //[RegularExpression(@"^[0-9]$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string numberPhone { set; get; }
        [Required(ErrorMessage = "Không được bỏ trống mật khẩu")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Mật khẩu phải trên 6 ký tự và dưới 50 ký tự")]
        public string passWord { set; get; }
        [Required(ErrorMessage = "Không được bỏ trống nhập lại mật khẩu mật khẩu")]
        [Compare("passWord", ErrorMessage = "Nhập lại mật khẩu không khớp")]
        public string comfirmPassword { set; get; }
    }
}
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WebBanHang.ModelViews
{
    public class RegisterVM
    {
        [Key]
        public int CustomerID { get; set; }

        [Display(Name ="Họ và tên")]
        [Required(ErrorMessage ="Vui lòng nhập họ tên")]
        public string FullName { get; set; }

        [MaxLength(250)]
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Vui lòng nhập email")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [Remote(action:"ValidateEmail", controller:"Accounts")]
        public string Email { get; set; }

        [MaxLength(10)]
        [Display(Name = "Số điện thoại")]
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [DataType(DataType.PhoneNumber)]
        [Remote(action: "ValidatePhone", controller: "Accounts")]
        public string Phone { get; set; }

        [MaxLength(50)]
        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [MinLength(6, ErrorMessage ="Mật khẩu tối thiểu 6 ký tự")]
        public string Password { get; set; }

        [MaxLength(50)]
        [Display(Name = "Nhập lại mật khẩu")]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [MinLength(6, ErrorMessage = "Mật khẩu tối thiểu 6 ký tự")]
        [Compare("Password", ErrorMessage ="Vui lòng nhập mật khẩu giống nhau")]
        public string ConfirmPassword { get; set; }

    }
}

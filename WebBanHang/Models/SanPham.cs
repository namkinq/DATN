using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace WebBanHang.Models
{
    public partial class SanPham
    {
        public SanPham()
        {
            ChiTietDonHangs = new HashSet<ChiTietDonHang>();
        }

        public int MaSp { get; set; }


        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        [MaxLength(250, ErrorMessage = "Tên sản phẩm không quá 250 kí tự")]
        public string TenSp { get; set; }


        [Required(ErrorMessage = "Giá bán không được để trống")]
        [Range(0, int.MaxValue, ErrorMessage = "Giá bán > 0")]
        public int? GiaBan { get; set; }


        [Required(ErrorMessage = "Giá giảm không được để trống")]
        [Range(0, int.MaxValue, ErrorMessage = "Giá giảm > 0")]
        public int? GiaGiam { get; set; }


        [Required(ErrorMessage = "Số lượng không được để trống")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng > 0")]
        public int? SoLuongCo { get; set; }


        //
        public string Anh { get; set; }


        [Range(0, int.MaxValue, ErrorMessage = "Công suất > 0")]
        public int? CongSuat { get; set; }


        //
        [Range(0, float.MaxValue, ErrorMessage = "Khối lượng > 0")]
        public double? KhoiLuong { get; set; }


        [MaxLength(500, ErrorMessage = "Mô tả không quá 500 kí tự")]
        public string MoTa { get; set; }


        [MaxLength(50, ErrorMessage = "Không quá 50 kí tự")]
        public string BaoHanh { get; set; }


        [Required(ErrorMessage = "Chưa chọn loại")]
        public int? MaLoai { get; set; }


        [Required(ErrorMessage = "Chưa chọn thương hiệu")]
        public int? MaTh { get; set; }


        public virtual LoaiSanPham MaLoaiNavigation { get; set; }
        public virtual ThuongHieu MaThNavigation { get; set; }
        public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; }
    }
}

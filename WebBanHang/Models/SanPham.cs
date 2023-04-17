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
        [Required(ErrorMessage="Tên sản phẩm không được để trống")]
        public string TenSp { get; set; }
        public int? GiaBan { get; set; }
        public int? GiaGiam { get; set; }
        public int? SoLuongCo { get; set; }
        public string Anh { get; set; }
        public int? CongSuat { get; set; }
        public double? KhoiLuong { get; set; }
        public string MoTa { get; set; }
        public string BaoHanh { get; set; }
        public int? MaLoai { get; set; }
        public int? MaTh { get; set; }

        public virtual LoaiSanPham MaLoaiNavigation { get; set; }
        public virtual ThuongHieu MaThNavigation { get; set; }
        public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; }
    }
}

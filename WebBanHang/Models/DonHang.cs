using System;
using System.Collections.Generic;

#nullable disable

namespace WebBanHang.Models
{
    public partial class DonHang
    {
        public DonHang()
        {
            ChiTietDonHangs = new HashSet<ChiTietDonHang>();
        }

        public int MaDh { get; set; }
        public DateTime? NgayDat { get; set; }
        public DateTime? NgayShip { get; set; }
        public int? TienShip { get; set; }
        public int? GiamGiaShip { get; set; }
        public int? GiamGia { get; set; }
        public int? TongTien { get; set; }
        public string DiaChi { get; set; }
        public string TrangThai { get; set; }
        public int? MaKh { get; set; }
        public int? MaShipper { get; set; }

        public virtual KhachHang MaKhNavigation { get; set; }
        public virtual Shipper MaShipperNavigation { get; set; }
        public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; }
    }
}

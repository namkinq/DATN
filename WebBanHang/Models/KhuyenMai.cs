using System;
using System.Collections.Generic;

#nullable disable

namespace WebBanHang.Models
{
    public partial class KhuyenMai
    {
        public KhuyenMai()
        {
            DonHangs = new HashSet<DonHang>();
        }

        public int MaKm { get; set; }
        public string MoTa { get; set; }
        public string MaNhap { get; set; }
        public int? SoLuong { get; set; }
        public int? GiaTriToiThieu { get; set; }
        public int? GiaTriGiam { get; set; }
        public DateTime? NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }

        public virtual ICollection<DonHang> DonHangs { get; set; }
    }
}

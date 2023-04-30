using System;
using System.Collections.Generic;

#nullable disable

namespace WebBanHang.Models
{
    public partial class Shipper
    {
        public Shipper()
        {
            DonHangs = new HashSet<DonHang>();
        }

        public int MaShipper { get; set; }
        public string TenShipper { get; set; }
        public string TenHt { get; set; }
        public string Email { get; set; }
        public string Sdt { get; set; }
        public string MatKhau { get; set; }
        public string LoaiXe { get; set; }
        public string BienSo { get; set; }
        public string Salt { get; set; }

        public virtual ICollection<DonHang> DonHangs { get; set; }
    }
}

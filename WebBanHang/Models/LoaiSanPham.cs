using System;
using System.Collections.Generic;

#nullable disable

namespace WebBanHang.Models
{
    public partial class LoaiSanPham
    {
        public LoaiSanPham()
        {
            SanPhams = new HashSet<SanPham>();
        }

        public int MaLoai { get; set; }
        public string TenLoai { get; set; }
        public string MoTa { get; set; }

        public virtual ICollection<SanPham> SanPhams { get; set; }
    }
}

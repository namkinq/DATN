using System;
using System.Collections.Generic;

#nullable disable

namespace WebBanHang.Models
{
    public partial class ThuongHieu
    {
        public ThuongHieu()
        {
            SanPhams = new HashSet<SanPham>();
        }

        public int MaTh { get; set; }
        public string TenTh { get; set; }
        public string MoTa { get; set; }

        public virtual ICollection<SanPham> SanPhams { get; set; }
    }
}

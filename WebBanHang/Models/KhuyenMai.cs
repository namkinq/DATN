using System;
using System.Collections.Generic;

#nullable disable

namespace WebBanHang.Models
{
    public partial class KhuyenMai
    {
        public int MaCtkm { get; set; }
        public string Ma { get; set; }
        public string LoaiKm { get; set; }
        public string MoTa { get; set; }
        public int? GttoiThieu { get; set; }
        public int? Gtgiam { get; set; }
        public DateTime? NgayBd { get; set; }
        public DateTime? NgayKt { get; set; }
    }
}

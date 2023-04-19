using System.Collections.Generic;
using WebBanHang.Models;

namespace WebBanHang.ModelViews
{
    public class ProductHomeVM
    {
        public LoaiSanPham category { get; set; }
        public List<SanPham> lsProducts { get; set; }
    }
}

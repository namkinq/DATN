using WebBanHang.Models;

namespace WebBanHang.ModelViews
{
    public class CartItem
    {
        public SanPham product { get; set; }
        public int amount { get; set; }
        public double TotalMoney => amount * product.GiaGiam.Value;
    }
}

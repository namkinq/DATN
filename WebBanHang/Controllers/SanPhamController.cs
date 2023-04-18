using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using WebBanHang.Models;

namespace WebBanHang.Controllers
{
    public class SanPhamController : Controller
    {
        private readonly dbBanHangContext _context;
        public SanPhamController(dbBanHangContext context)
        {
            _context = context;
        }
        [Route("sanpham")]
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Detail(int id)
        {
            //include???
            var product = _context.SanPhams.Include(x=>x.MaLoaiNavigation).FirstOrDefault(x => x.MaSp == id);
            if (product == null)
            {
                return RedirectToAction("Index");
            }
            return View(product);
        }
    }
}

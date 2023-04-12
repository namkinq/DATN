using Microsoft.AspNetCore.Mvc;
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
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Detail(int id)
        {
            var product = _context.SanPhams.FirstOrDefault(x => x.MaSp == id);
            if (product == null)
            {
                return RedirectToAction("Index");
            }
            return View(product);
        }
    }
}

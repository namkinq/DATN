using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WebBanHang.Models;

namespace WebBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SearchController : Controller
    {
        private readonly dbBanHangContext _context;

        public SearchController(dbBanHangContext context)
        {
            _context = context;
        }

        // GET: Search/TimSanPham
        public IActionResult TimSanPham(string searchKey)
        {
            List<SanPham> ls = new List<SanPham>();
            if (string.IsNullOrEmpty(searchKey) || searchKey.Length < 1)
            {
                return PartialView("ListSanPhamSearchPartial", null);
            }

            ls = _context.SanPhams
                .AsNoTracking()
                .Include(x=>x.MaLoaiNavigation)
                .Include(x=>x.MaThNavigation)
                .Where(x=>x.TenSp.Contains(searchKey))
                .OrderByDescending(x=>x.TenSp)
                .Take(10)
                .ToList();
            if (ls == null)
            {
                return PartialView("ListSanPhamSearchPartial", null);
            }
            else
            {
                return PartialView("ListSanPhamSearchPartial", ls);

            }
        }
    }
}

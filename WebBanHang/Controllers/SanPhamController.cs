using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
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
        public IActionResult Index(int? page)
        {
            try
            {
                var pageNumber = page == null || page <= 0 ? 1 : page.Value;
                var pageSize = 9;

                var lsSanPham = _context.SanPhams
                    .AsNoTracking()
                    .Include(s => s.MaLoaiNavigation)
                    .Include(s => s.MaThNavigation)
                    .OrderByDescending(x => x.MaSp).ToList();

                PagedList<SanPham> models = new PagedList<SanPham>(lsSanPham.AsQueryable(), pageNumber, pageSize);
                ViewBag.CurrentPage = pageNumber;

                //
                var dmsp = _context.LoaiSanPhams
                    .AsNoTracking()
                    .OrderBy(x => x.MaLoai).ToList();
                ViewBag.DMSP = dmsp;
                //

                return View(models);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
            
        }
        [Route("sanphams/{MaLoai}")]
        public IActionResult List(int MaLoai, int page = 1)
        {
            try
            {
                //
                var dmsp = _context.LoaiSanPhams
                    .AsNoTracking()
                    .OrderBy(x => x.MaLoai).ToList();
                ViewBag.DMSP = dmsp;
                //

                var pageNumber = page;
                var pageSize = 9;
                var danhmuc = _context.LoaiSanPhams
                    .AsNoTracking()
                    .SingleOrDefault(x => x.MaLoai == MaLoai);

                var lsSanPham = _context.SanPhams
                    .AsNoTracking()
                    .Where(x => x.MaLoai == MaLoai)
                    .Include(s => s.MaLoaiNavigation)
                    .Include(s => s.MaThNavigation)
                    .OrderByDescending(x => x.MaSp).ToList();

                PagedList<SanPham> models = new PagedList<SanPham>(lsSanPham.AsQueryable(), pageNumber, pageSize);
                ViewBag.CurrentPage = pageNumber;
                ViewBag.CurrentLoaiSP = danhmuc;

                return View(models);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }

        }
        [Route("sanpham/{id}")]
        public IActionResult Detail(int id)
        {
            try
            {
                var product = _context.SanPhams
                .Include(x => x.MaLoaiNavigation)
                .Include(x => x.MaThNavigation)
                .FirstOrDefault(x => x.MaSp == id);

                if (product == null)
                {
                    return RedirectToAction("Index");
                }

                var lsProduct = _context.SanPhams
                    .AsNoTracking()
                    .Where(x=>x.MaLoai==product.MaLoai &&x.MaSp!=id)
                    .Take(4)
                    .OrderByDescending(x=>x.MaSp)
                    .ToList();
                ViewBag.SanPham = lsProduct;

                return View(product);
            }
            catch
            {
                return RedirectToAction("Index", "Home");

            }

        }
    }
}

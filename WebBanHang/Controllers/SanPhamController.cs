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
        public IActionResult Index(int? page, string? searchKey, int? sort)
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

                if (searchKey != null)
                {
                    lsSanPham = lsSanPham.Where(x=>x.TenSp.ToLower().Contains(searchKey.ToLower())).ToList();
                }
                if(sort!= null)
                {
                    if (sort == 1)
                    {
                        lsSanPham = lsSanPham.OrderBy(x => x.GiaGiam).ToList();
                    }
                    else if(sort==2)
                    {
                        lsSanPham = lsSanPham.OrderByDescending(x => x.GiaGiam).ToList();
                    }
                }

                PagedList<SanPham> models = new PagedList<SanPham>(lsSanPham.AsQueryable(), pageNumber, pageSize);
                ViewBag.CurrentPage = pageNumber;

                //
                var dmsp = _context.LoaiSanPhams
                    .AsNoTracking()
                    .OrderBy(x => x.MaLoai).ToList();
                ViewBag.DMSP = dmsp;
                //
                var spmoi = _context.SanPhams
                    .AsNoTracking()
                    .Include(s => s.MaLoaiNavigation)
                    .Include(s => s.MaThNavigation)
                    .OrderByDescending(x => x.MaSp)
                    .Take(3)
                    .ToList();
                ViewBag.SPM = spmoi;
                //
                return View(models);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
            
        }

        [Route("sanpham/sort")]
        public IActionResult Sort(int sort)
        {
            var url = $"/sanpham?sort={sort}";
            if (sort == 0)
            {
                url = $"/sanpham";
            }
            else
            {
                //if(maLoai==0) url = $"/Admin/SanPhams?maTh={maTh}&stt={stt}";
            }
            return Json(new { status = "success", RedirectUrl = url });
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

                var lsDG =_context.DanhGiaSanPhams
                    .Include(x=>x.MaKhNavigation)
                    .AsNoTracking()
                    .Where(x=>x.MaSp==id)
                    .OrderByDescending(x=>x.MaDg)
                    .ToList();
                ViewBag.DanhGia = lsDG;

                return View(product);
            }
            catch
            {
                return RedirectToAction("Index", "Home");

            }

        }
    }
}

using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebBanHang.Models;
using WebBanHang.ModelViews;

namespace WebBanHang.Controllers
{
    public class DonHangController : Controller
    {
        private readonly dbBanHangContext _context;
        public INotyfService _notyfService { get; }

        public DonHangController(dbBanHangContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Details(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            try
            {
                var taikhoanID = HttpContext.Session.GetString("CustomerId");
                if (string.IsNullOrEmpty(taikhoanID))
                {
                    return RedirectToAction("Login", "Accounts");
                }
                var khachhang = _context.KhachHangs.AsNoTracking()
                    .SingleOrDefault(x => x.MaKh == Convert.ToInt32(taikhoanID));
                if (khachhang == null)
                {
                    return NotFound();
                }
                var donhang = await _context.DonHangs
                    .FirstOrDefaultAsync(m => m.MaDh == id && Convert.ToInt32(taikhoanID) == m.MaKh);
                if(donhang == null)
                {
                    return NotFound();
                }

                var ctdh = _context.ChiTietDonHangs
                    .AsNoTracking()
                    .Where(x => x.MaDh == id)
                    .Include(x => x.MaSpNavigation)
                    .OrderBy(x => x.MaSp)
                    .ToList();

                XemDonHang donHang = new XemDonHang();
                donHang.DonHang = donhang;
                donHang.ChiTietDonHang = ctdh;

                return PartialView("Details", donHang);
            }
            catch
            {
                return NotFound();
            }
            
        }

        [HttpPost]
        public async Task<IActionResult> Huy(int? madh)
        {
            if (madh == null)
            {
                return RedirectToAction("Dashboard", "Accounts");
            }
            try
            {
                var taikhoanID = HttpContext.Session.GetString("CustomerId");
                if (string.IsNullOrEmpty(taikhoanID))
                {
                    return RedirectToAction("Login", "Accounts");
                }
                var khachhang = _context.KhachHangs.AsNoTracking()
                    .SingleOrDefault(x => x.MaKh == Convert.ToInt32(taikhoanID));
                if (khachhang == null)
                {
                    return NotFound();
                }

                var donhang = await _context.DonHangs
                    .FirstOrDefaultAsync(m => m.MaDh == madh && Convert.ToInt32(taikhoanID) == m.MaKh);
                if (donhang == null)
                {
                    return RedirectToAction("Dashboard", "Accounts");
                }
                if (donhang.TrangThai != "Chờ xử lý")
                {
                    _notyfService.Warning("Hủy thất bại");
                    return RedirectToAction("Dashboard", "Accounts");
                }
                donhang.TrangThai = "Đã hủy";

                _context.Update(donhang);
                _context.SaveChanges();

                _notyfService.Success("Hủy thành công");

                return RedirectToAction("Dashboard", "Accounts");

            }
            catch
            {
                return RedirectToAction("Dashboard", "Accounts");
            }

        }

        //[HttpPost]
        //public async Task<IActionResult> DanhGia(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }
        //    try
        //    {
        //        var taikhoanID = HttpContext.Session.GetString("CustomerId");
        //        if (string.IsNullOrEmpty(taikhoanID))
        //        {
        //            return RedirectToAction("Login", "Accounts");
        //        }
        //        var khachhang = _context.KhachHangs.AsNoTracking()
        //            .SingleOrDefault(x => x.MaKh == Convert.ToInt32(taikhoanID));
        //        if (khachhang == null)
        //        {
        //            return NotFound();
        //        }
        //        var donhang = await _context.DonHangs
        //            .FirstOrDefaultAsync(m => m.MaDh == id && Convert.ToInt32(taikhoanID) == m.MaKh);
        //        if (donhang == null)
        //        {
        //            return NotFound();
        //        }

        //        var ctdh = _context.ChiTietDonHangs
        //            .AsNoTracking()
        //            .Where(x => x.MaDh == id)
        //            .Include(x => x.MaSpNavigation)
        //            .OrderBy(x => x.MaSp)
        //            .ToList();

        //        XemDonHang donHang = new XemDonHang();
        //        donHang.DonHang = donhang;
        //        donHang.ChiTietDonHang = ctdh;

        //        return PartialView("Details", donHang);
        //    }
        //    catch
        //    {
        //        return NotFound();
        //    }

        //}
    }
}

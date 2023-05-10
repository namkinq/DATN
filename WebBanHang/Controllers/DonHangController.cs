﻿using AspNetCoreHero.ToastNotification.Abstractions;
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

        public string getLocation(string maxa, string maqh, string matp)
        {
            try
            {
                var xa = _context.XaPhuongThiTrans.AsNoTracking()
                    .SingleOrDefault(x => x.Maxa == maxa);
                var qh = _context.QuanHuyens.AsNoTracking()
                    .SingleOrDefault(x => x.Maqh == maqh);
                var tp = _context.TinhThanhPhos.AsNoTracking()
                    .SingleOrDefault(x => x.Matp == matp);

                if (xa != null && qh != null && tp != null)
                {
                    return $"{xa.Name}, {qh.Name}, {tp.Name}";
                }
            }
            catch
            {
                return string.Empty;
            }
            return string.Empty;
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
                    return RedirectToAction("DangNhap", "Accounts");
                }
                var khachhang = _context.KhachHangs.AsNoTracking()
                    .SingleOrDefault(x => x.MaKh == Convert.ToInt32(taikhoanID));
                if (khachhang == null)
                {
                    return NotFound();
                }
                var donhang = await _context.DonHangs
                    .Include(x=>x.MaTtNavigation)
                    .Include(x=>x.MaKhNavigation)
                    .Include(x=>x.MaShipperNavigation)
                    .Include(x=>x.DanhGiaSanPhams)
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

                string fullAddress = $"{donhang.DiaChi}, {getLocation(donhang.Maxa, donhang.Maqh, donhang.Matp)}";
                ViewBag.FullAddress = fullAddress;

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
                    return RedirectToAction("DangNhap", "Accounts");
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
                if (donhang.MaTt != 1)
                {
                    _notyfService.Warning("Hủy thất bại");
                    return RedirectToAction("Dashboard", "Accounts");
                }
                donhang.MaTt = 6;

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

        [HttpPost]
        public async Task<IActionResult> DanhGia(int? madh, int masp, byte diem, string noiDung)
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
                    return RedirectToAction("DangNhap", "Accounts");
                }
                var khachhang = _context.KhachHangs.AsNoTracking()
                    .SingleOrDefault(x => x.MaKh == Convert.ToInt32(taikhoanID));
                if (khachhang == null)
                {
                    return NotFound();
                }
                //
                var donhang = await _context.DonHangs
                    .FirstOrDefaultAsync(m => m.MaDh == madh && Convert.ToInt32(taikhoanID) == m.MaKh);
                if (donhang == null)
                {
                    return RedirectToAction("Dashboard", "Accounts");
                }
                //
                DanhGiaSanPham dgsp = new DanhGiaSanPham();
                dgsp.MaKh = khachhang.MaKh;
                dgsp.MaDh = donhang.MaDh;
                dgsp.MaSp = masp;
                dgsp.Diem = diem;
                dgsp.NoiDung = noiDung;
                dgsp.ThoiGian = DateTime.Now;

                _context.Add(dgsp);
                _context.SaveChanges();

                _notyfService.Success("Đánh giá thành công");

                return RedirectToAction("Dashboard", "Accounts");

            }
            catch
            {
                return RedirectToAction("Dashboard", "Accounts");
            }

        }
    }
}

using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WebBanHang.Areas.Admin.Controllers;
using WebBanHang.Extension;
using WebBanHang.Models;
using WebBanHang.ModelViews;

namespace WebBanHang.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly dbBanHangContext _context;
        public INotyfService _notyfService { get; }

        public CheckoutController(dbBanHangContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        public List<CartItem> GioHang
        {
            get
            {
                var gh = HttpContext.Session.Get<List<CartItem>>("GioHang");
                if (gh == default(List<CartItem>))
                {
                    gh = new List<CartItem>();
                }
                return gh;
            }
        }
        [Route("checkout")]
        public IActionResult Index(string returnUrl = null)
        {
            //lấy giỏ
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            var taikhoanID = HttpContext.Session.GetString("CustomerId");
            MuaHangVM model = new MuaHangVM();

            if (taikhoanID != null)
            {
                var khachhang = _context.KhachHangs.AsNoTracking()
                    .SingleOrDefault(x => x.MaKh == Convert.ToInt32(taikhoanID));
                model.CustomerId = khachhang.MaKh;
                model.FullName = khachhang.TenKh;
                model.Email = khachhang.Email;
                model.Phone = khachhang.Sdt;
                model.Address = khachhang.DiaChi;
            }
            ViewBag.GioHang = cart;
            return View(model);
        }

        [HttpPost]
        [Route("checkout")]
        public IActionResult Index(MuaHangVM muahang)
        {
            //lấy giỏ
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            var taikhoanID = HttpContext.Session.GetString("CustomerId");
            MuaHangVM model = new MuaHangVM();

            if (taikhoanID != null)
            {
                var khachhang = _context.KhachHangs.AsNoTracking()
                    .SingleOrDefault(x => x.MaKh == Convert.ToInt32(taikhoanID));
                model.CustomerId = khachhang.MaKh;
                model.FullName = khachhang.TenKh;
                model.Email = khachhang.Email;
                model.Phone = khachhang.Sdt;
                model.Address = khachhang.DiaChi;

                khachhang.DiaChi = muahang.Address;

                _context.Update(khachhang);
                _context.SaveChanges();
            }
            ViewBag.GioHang = cart;
            try
            {
                if (ModelState.IsValid)
                {
                    //khoi tao
                    DonHang donhang = new DonHang();

                    donhang.MaKh = model.CustomerId;
                    donhang.DiaChi = model.Address;

                    donhang.NgayDat = DateTime.Now;
                    donhang.TrangThai = "Chờ xử lý";

                    _context.Add(donhang);
                    _context.SaveChanges();

                    //ds sp
                    foreach (var item in cart)
                    {
                        ChiTietDonHang ctdh = new ChiTietDonHang();
                        ctdh.MaDh = donhang.MaDh;
                        ctdh.MaSp = item.product.MaSp;
                        ctdh.GiaBan = item.product.GiaBan;
                        ctdh.GiaGiam = item.product.GiaGiam;
                        ctdh.SoLuong = item.amount;

                        _context.Add(ctdh);
                    }
                    _context.SaveChanges();

                    //clear
                    HttpContext.Session.Remove("GioHang");
                    _notyfService.Success("Đặt hàng thành công");

                    return RedirectToAction("Dashboard", "Accounts");
                }
            }
            catch
            {
                ViewBag.GioHang = cart;
                return View(model);
            }

            ViewBag.GioHang = cart;
            return View(model);
        }

        //[Route("dat-hang-thanh-cong")]
        //public IActionResult Success()
        //{

        //    try
        //    {
        //        var taikhoanID = HttpContext.Session.GetString("CustomerId");
        //        if(string.IsNullOrEmpty(taikhoanID))
        //        {
        //            return RedirectToAction("Login", "Accounts", new { returnUrl = "/dat-hang-thanh-cong" });
        //        }
        //        var khachhang = _context.KhachHangs.AsNoTracking()
        //            .SingleOrDefault(x => x.MaKh == Convert.ToInt32(taikhoanID));
        //        var donhang = _context.DonHangs.Where(x=>x.MaKh == Convert.ToInt32(taikhoanID))
        //            .OrderByDescending(x=>x.MaDh)
        //            .FirstOrDefault();


        //    }
        //    catch
        //    {

        //    }
        //}
    }
}

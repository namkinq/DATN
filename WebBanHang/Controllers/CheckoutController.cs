using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
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
            if (string.IsNullOrEmpty(taikhoanID))
            {
                return RedirectToAction("DangNhap", "Accounts");
            }
            MuaHangVM model = new MuaHangVM();

            if (taikhoanID != null)
            {
                var khachhang = _context.KhachHangs.AsNoTracking()
                    .SingleOrDefault(x => x.MaKh == Convert.ToInt32(taikhoanID));
                model.CustomerId = khachhang.MaKh;
                model.FullName = khachhang.TenKh;
                model.Email = khachhang.Email;
                model.Phone = khachhang.Sdt;

                if (khachhang.DiaChi != null)
                {
                    model.Address = khachhang.DiaChi;
                }
                //tp-qh-xp
                if (khachhang.Matp != null)
                {
                    ViewData["lsTinhThanh"] = new SelectList(_context.TinhThanhPhos.OrderBy(x => x.Matp).ToList(), "Matp", "Name", khachhang.Matp);
                    if (khachhang.Maqh != null)
                    {
                        ViewData["lsQuanHuyen"] = new SelectList(_context.QuanHuyens.Where(x => x.Matp == khachhang.Matp).OrderBy(x => x.Maqh).ToList(), "Maqh", "Name", khachhang.Maqh);
                        if (khachhang.Maxa != null)
                        {
                            ViewData["lsPhuongXa"] = new SelectList(_context.XaPhuongThiTrans.Where(x => x.Maqh == khachhang.Maqh).OrderBy(x => x.Maxa).ToList(), "Maxa", "Name", khachhang.Maxa);
                        }
                        else
                        {
                            ViewData["lsPhuongXa"] = new SelectList(_context.XaPhuongThiTrans.Where(x => x.Maqh == khachhang.Maqh).OrderBy(x => x.Maxa).ToList(), "Maxa", "Name");
                        }
                    }
                    else
                    {
                        ViewData["lsQuanHuyen"] = new SelectList(_context.QuanHuyens.Where(x=>x.Matp==khachhang.Matp).OrderBy(x => x.Maqh).ToList(), "Maqh", "Name");
                    }
                }
                else
                {
                    ViewData["lsTinhThanh"] = new SelectList(_context.TinhThanhPhos.OrderBy(x => x.Matp).ToList(), "Matp", "Name");
                } 
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
                //model.TinhThanh = khachhang.Matp;

                if (khachhang.DiaChi == null) khachhang.DiaChi = muahang.Address;
                if (khachhang.Matp == null) khachhang.Matp = muahang.TinhThanh;
                if (khachhang.Maqh == null) khachhang.Maqh = muahang.QuanHuyen;
                if (khachhang.Maxa == null) khachhang.Maxa = muahang.PhuongXa;


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
                    donhang.Matp = model.TinhThanh;
                    donhang.Maqh = model.QuanHuyen;
                    donhang.Maxa = model.PhuongXa;

                    donhang.NgayDat = DateTime.Now;
                    donhang.MaTt = 1;

                    donhang.TongTien = Convert.ToInt32(cart.Sum(x => x.TotalMoney));

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
                        ctdh.TongTien = item.TotalMoney;
                        //
                        SanPham hh = _context.SanPhams.SingleOrDefault(p => p.MaSp == item.product.MaSp);
                        hh.SoLuongCo -= 1;

                        _context.Add(ctdh);
                        _context.Update(hh);
                    }

                    _context.SaveChanges();

                    //

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

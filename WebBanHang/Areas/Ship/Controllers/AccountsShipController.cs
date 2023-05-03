﻿using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WebBanHang.Extension;
using WebBanHang.Helper;
using WebBanHang.Models;
using WebBanHang.ModelViews;

namespace WebBanHang.Areas.Ship.Controllers
{
    [Area("Ship")]
    public class AccountsShipController : Controller
    {
        private readonly dbBanHangContext _context;
        public INotyfService _notyfService { get; }
        public AccountsShipController(dbBanHangContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        //[AllowAnonymous]
        //[Route("dangnhap")]
        public IActionResult DangNhap()
        {
            var taikhoanID = HttpContext.Session.GetString("ShipId");
            if (taikhoanID != null)
            {

                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        //[AllowAnonymous]
        //[Route("dangnhap")]
        public async Task<IActionResult> DangNhap(LoginAdminVM customer, string returnUrl = null)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isEmail = Utilities.IsValidEmail(customer.UserName);
                    if (!isEmail) return View(customer);

                    var khachhang = _context.Shippers.AsNoTracking()
                        .SingleOrDefault(x => x.Email.Trim() == customer.UserName);

                    if (khachhang == null)
                    {
                        _notyfService.Warning("Thông tin đăng nhập không chính xác");
                        return View(customer);
                    }


                    string pass = (customer.Password + khachhang.Salt.Trim()).ToMD5();
                    if (khachhang.MatKhau != pass)
                    {
                        _notyfService.Warning("Thông tin đăng nhập không chính xác");
                        return View(customer);
                    }

                    // lưu session
                    HttpContext.Session.SetString("ShipId", khachhang.MaShipper.ToString());
                    var taikhoanID = HttpContext.Session.GetString("ShipId");

                    //Identity?
                    //var claims = new List<Claim>
                    //    {
                    //        new Claim(ClaimTypes.Name, khachhang.TenKh),
                    //        new Claim("CustomerId", khachhang.MaKh.ToString())
                    //    };
                    //ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");
                    //ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    //await HttpContext.SignInAsync(claimsPrincipal);

                    _notyfService.Success("Đăng nhập thành công");
                    return RedirectToAction("Index", "Home");

                }
            }
            catch
            {
                return View(customer);

            }
            return View(customer);
        }
        [HttpGet]
        public IActionResult DangXuat()
        {
            //HttpContext.SignOutAsync();
            HttpContext.Session.Remove("ShipId");
            return RedirectToAction("DangNhap");
        }
    }
}

using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebBanHang.Extension;
using WebBanHang.Helper;
using WebBanHang.Models;
using WebBanHang.ModelViews;

namespace WebBanHang.Controllers
{
    [Authorize]
    public class AccountsController : Controller
    {
        private readonly dbBanHangContext _context;
        public INotyfService _notyfService { get; }
        public AccountsController(dbBanHangContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult ValidatePhone(string Phone)
        {
            try
            {
                var kh = _context.KhachHangs.AsNoTracking()
                    .SingleOrDefault(x => x.Sdt.ToLower() == Phone);
                if (kh != null)
                {
                    return Json(false);
                }
                else return Json(true);
            }
            catch
            {
                return Json(false);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult ValidateEmail(string Email)
        {
            try
            {
                var kh = _context.KhachHangs.AsNoTracking()
                    .SingleOrDefault(x => x.Email.ToLower() == Email);
                if (kh != null)
                {
                    return Json(false);
                }
                else return Json(true);
            }
            catch
            {
                return Json(false);
            }
        }

        [Route("taikhoan", Name = "TaiKhoanCuaToi")]
        public IActionResult Dashboard()
        {
            var taikhoanID = HttpContext.Session.GetString("CustomerId");
            if (taikhoanID != null)
            {
                var khachhang = _context.KhachHangs.AsNoTracking()
                    .SingleOrDefault(x => x.MaKh == Convert.ToInt32(taikhoanID));
                if (khachhang != null)
                {
                    var lsDonHang = _context.DonHangs
                        .Include(x => x.ChiTietDonHangs)
                        .Include(x => x.MaTtNavigation)
                        .AsNoTracking()
                        .Where(x => x.MaKh == khachhang.MaKh)
                        .OrderByDescending(x => x.NgayDat).ToList();

                    ViewBag.DonHang = lsDonHang;

                    return View(khachhang);
                }

            }

            return RedirectToAction("DangNhap");
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("dangky", Name = "DangKy")]
        public IActionResult DangKyTaiKhoan()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("dangky", Name = "DangKy")]
        public async Task<IActionResult> DangKyTaiKhoan(RegisterVM taikhoan)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string salt = Utilities.GetRandomKey();
                    KhachHang kh = new KhachHang
                    {
                        TenKh = taikhoan.FullName,
                        Sdt = taikhoan.Phone.Trim().ToLower(),
                        Email = taikhoan.Email.Trim().ToLower(),
                        MatKhau = (taikhoan.Password + salt.Trim()).ToMD5(),
                        Khoa = false,
                        Salt = salt
                    };
                    try
                    {
                        _context.Add(kh);
                        await _context.SaveChangesAsync();
                        //lưu session
                        HttpContext.Session.SetString("CustomerId", kh.MaKh.ToString());
                        var taikhoanID = HttpContext.Session.GetString("CustomerId");
                        //Identity
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, kh.TenKh),
                            new Claim("CustomerId", kh.MaKh.ToString())
                        };
                        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");
                        ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                        await HttpContext.SignInAsync(claimsPrincipal);

                        _notyfService.Success("Đăng ký thành công");

                        return RedirectToAction("Dashboard", "Accounts");
                    }
                    catch
                    {
                        return RedirectToAction("DangKyTaiKhoan", "Accounts");
                    }
                }
                else
                {
                    return View(taikhoan);
                }
            }
            catch
            {
                return View(taikhoan);

            }
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("dangnhap", Name = "DangNhap")]
        public IActionResult DangNhap(string returnUrl = null)
        {
            var taikhoanID = HttpContext.Session.GetString("CustomerId");
            if (taikhoanID != null)
            {

                return RedirectToAction("Dashboard", "Accounts");
            }

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("dangnhap", Name = "DangNhap")]
        public async Task<IActionResult> DangNhap(LoginViewModel customer, string returnUrl = null)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isEmail = Utilities.IsValidEmail(customer.UserName);
                    if (!isEmail) return View(customer);

                    var khachhang = _context.KhachHangs.AsNoTracking()
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
                    if (khachhang.Khoa == true)
                    {
                        _notyfService.Error("Tài khoản bị khóa");
                        return View(customer);
                    }

                    // lưu session
                    HttpContext.Session.SetString("CustomerId", khachhang.MaKh.ToString());
                    var taikhoanID = HttpContext.Session.GetString("CustomerId");

                    //Identity
                    var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, khachhang.TenKh),
                            new Claim("CustomerId", khachhang.MaKh.ToString())
                        };
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");
                    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync(claimsPrincipal);

                    _notyfService.Success("Đăng nhập thành công");
                    return RedirectToAction("Dashboard", "Accounts");

                }
            }
            catch
            {
                return RedirectToAction("DangKyTaiKhoan", "Accounts");

            }
            return View(customer);
        }
        [HttpGet]
        [Route("dangxuat", Name = "DangXuat")]
        public IActionResult DangXuat()
        {
            HttpContext.SignOutAsync();
            HttpContext.Session.Remove("CustomerId");
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult ChangeInfo(ChangeInfoVM model)
        {
            try
            {
                var taikhoanID = HttpContext.Session.GetString("CustomerId");
                if (taikhoanID == null)
                {
                    return RedirectToAction("DangNhap", "Accounts");
                }

                if (ModelState.IsValid)
                {
                    var taikhoan = _context.KhachHangs.Find(Convert.ToInt32(taikhoanID));
                    if (taikhoan == null) return RedirectToAction("DangNhap", "Accounts");

                    var pass = (model.PasswordNow.Trim() + taikhoan.Salt.Trim()).ToMD5();
                    if (pass == taikhoan.MatKhau)
                    {
                        if (model.Password != null)
                        {
                            string passnew = (model.Password.Trim() + taikhoan.Salt.Trim()).ToMD5();
                            taikhoan.MatKhau = passnew;
                        }

                        taikhoan.TenKh = model.FullName;
                        taikhoan.DiaChi = model.Address;

                        _context.Update(taikhoan);
                        _context.SaveChanges();
                        _notyfService.Success("Cập nhật thành công");

                        return RedirectToAction("Dashboard", "Accounts");
                    }
                }

            }
            catch
            {
                _notyfService.Warning("Cập nhật không thành công");
                return RedirectToAction("Dashboard", "Account");
            }
            _notyfService.Warning("Cập nhật không thành công");
            return RedirectToAction("Dashboard", "Account");
        }
    }
}

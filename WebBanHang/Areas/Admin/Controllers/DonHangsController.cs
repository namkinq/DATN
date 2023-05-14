﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using WebBanHang.Models;
using DinkToPdf;
using DinkToPdf.Contracts;

namespace WebBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DonHangsController : Controller
    {
        private readonly dbBanHangContext _context;

        public INotyfService _notyfService { get; }
        private readonly IConverter _pdfConverter;

        public DonHangsController(dbBanHangContext context, INotyfService notyfService, IConverter pdfConverter)
        {
            _context = context;
            _notyfService = notyfService;
            _pdfConverter = pdfConverter;

        }

        // GET: Admin/DonHangs
        public IActionResult Index(int page = 1, int TrangThai = 0)
        {
            var taikhoanID = HttpContext.Session.GetString("AdminId");
            if (string.IsNullOrEmpty(taikhoanID))
            {
                return RedirectToAction("DangNhap", "AccountsAdmin");
            }

            var pageNumber = page;
            var pageSize = 10;

            List<DonHang> lsDonHang = new List<DonHang>();

            //filter op
            if (TrangThai != 0)
            {
                lsDonHang = _context.DonHangs
                .AsNoTracking()
                .Include(x => x.MaTtNavigation)
                .Where(x => x.MaTt == TrangThai)
                .OrderByDescending(x => x.MaDh).ToList();
            }
            else
            {
                lsDonHang = _context.DonHangs
                .AsNoTracking()
                .Include(x => x.MaTtNavigation)
                .OrderByDescending(x => x.MaDh).ToList();
            }

            //page
            PagedList<DonHang> models = new PagedList<DonHang>(lsDonHang.AsQueryable(), pageNumber, pageSize);

            ViewBag.CurrentPage = pageNumber;
            ViewBag.CurrentTrangThai = TrangThai;

            //lấy slted value
            ViewData["lsTrangThai"] = new SelectList(_context.TrangThaiDonHangs, "MaTt", "TenTt", TrangThai);

            return View(models);
        }
        public IActionResult Filter(int TrangThai = 0)
        {
            var url = $"/Admin/DonHangs?TrangThai={TrangThai}";
            if (TrangThai == 0)
            {
                url = $"/Admin/DonHangs";
            }
            else
            {
                //if(maLoai==0) url = $"/Admin/SanPhams?maTh={maTh}&stt={stt}";
            }
            return Json(new { status = "success", RedirectUrl = url });
        }

        // GET: Admin/DonHangs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var donHang = await _context.DonHangs
                .Include(d => d.MaKhNavigation)
                .Include(d => d.MaShipperNavigation)
                .FirstOrDefaultAsync(m => m.MaDh == id);
            if (donHang == null)
            {
                return NotFound();
            }

            return View(donHang);
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
        //
        public async Task<IActionResult> ChangeStatus(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var donHang = await _context.DonHangs
                .Include(d => d.MaKhNavigation)
                .Include(d => d.MaShipperNavigation)
                .Include(d => d.MaTtNavigation)
                .Include(d => d.ChiTietDonHangs)
                .FirstOrDefaultAsync(m => m.MaDh == id);

            var ctdh = _context.ChiTietDonHangs
                .AsNoTracking()
                .Include(x => x.MaSpNavigation)
                .Where(x => x.MaDh == donHang.MaDh)
                .OrderBy(x => x.MaSp)
                .ToList();

            string fullAddress = $"{donHang.DiaChi}, {getLocation(donHang.Maxa, donHang.Maqh, donHang.Matp)}";
            ViewBag.FullAddress = fullAddress;

            ViewBag.ChiTiet = ctdh;
            if (donHang == null)
            {
                return NotFound();
            }

            ViewData["Shipper"] = new SelectList(_context.Shippers, "MaShipper", "TenHt", donHang.MaShipper);

            ViewData["lsTrangThai"] = new SelectList(_context.TrangThaiDonHangs, "MaTt", "TenTt", donHang.MaTt);

            return View(donHang);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeStatus(int id, DonHang donHang)
        {
            if (id != donHang.MaDh)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var donhang = await _context.DonHangs
                        .Include(d => d.MaKhNavigation)
                        .Include(d => d.MaTtNavigation)
                        .Include(d => d.ChiTietDonHangs)
                        .Include(d => d.MaShipperNavigation)
                        .FirstOrDefaultAsync(m => m.MaDh == id);

                    var ctdh = _context.ChiTietDonHangs
                        .AsNoTracking()
                        .Include(x => x.MaSpNavigation)
                        .Where(x => x.MaDh == donHang.MaDh)
                        .OrderBy(x => x.MaSp)
                        .ToList();

                    string fullAddress = $"{donhang.DiaChi}, {getLocation(donhang.Maxa, donhang.Maqh, donhang.Matp)}";
                    ViewBag.FullAddress = fullAddress;

                    ViewBag.ChiTiet = ctdh;

                    if (donhang != null)
                    {
                        if (donHang.MaTt == 3)
                        {
                            if (donHang.MaShipper == null)
                            {
                                _notyfService.Warning("Chưa chọn shipper");

                                ViewData["Shipper"] = new SelectList(_context.Shippers, "MaShipper", "TenHt", donHang.MaShipper);
                                ViewData["lsTrangThai"] = new SelectList(_context.TrangThaiDonHangs, "MaTt", "TenTt", donhang.MaTt);
                                return View(donhang);
                            }
                            else
                            {
                                donhang.MaTt = donHang.MaTt;
                                donhang.MaShipper = donHang.MaShipper;
                                donhang.NgayShip = DateTime.Now;
                            }
                        }
                        else
                        {
                            donhang.MaTt = donHang.MaTt;
                        }
                    }
                    _context.Update(donhang);
                    await _context.SaveChangesAsync();
                    _notyfService.Success("Cập nhật trạng thái thành công");

                    if (donHang.MaTt == 2)
                    {
                        ViewData["Shipper"] = new SelectList(_context.Shippers, "MaShipper", "TenHt", 0);
                    }
                    else
                    {
                        ViewData["Shipper"] = new SelectList(_context.Shippers, "MaShipper", "TenHt", donhang.MaShipper);
                    }

                    ViewData["lsTrangThai"] = new SelectList(_context.TrangThaiDonHangs, "MaTt", "TenTt", donhang.MaTt);
                    return RedirectToAction("ChangeStatus", id = donhang.MaDh);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DonHangExists(donHang.MaDh))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            //???
            ViewData["Shipper"] = new SelectList(_context.Shippers, "MaShipper", "TenHt", donHang.MaShipper);
            ViewData["lsTrangThai"] = new SelectList(_context.TrangThaiDonHangs, "MaTt", "TenTt", donHang.MaTt);

            return View(donHang);
        }

        public ActionResult InHoaDon(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dh = _context.DonHangs
                .AsNoTracking()
                .Include(d => d.MaKhNavigation)
                .Include(d => d.MaShipperNavigation)
                .Include(d => d.MaTtNavigation)
                .Include(d => d.ChiTietDonHangs)
                .FirstOrDefault(m => m.MaDh == id);

            var ctdh = _context.ChiTietDonHangs
                        .AsNoTracking()
                        .Include(x => x.MaSpNavigation)
                        .Where(x => x.MaDh == dh.MaDh)
                        .OrderBy(x => x.MaSp)
                        .ToList();
            if (dh == null)
            {
                return NotFound();
            }
            string fullAddress = $"{dh.DiaChi}, {getLocation(dh.Maxa, dh.Maqh, dh.Matp)}";
            // Get the HTML content of the invoice
            string html1 = $"";
            html1 +=
                $"<h4>Cửa hàng Yoko</h4>" +
                $"<h4>Địa chỉ: 36 Nguyên Xá, Minh Khai, Bắc Từ Liêm, Hà Nội</h4>" +
                $"<html><body><h1 style=\"text-align:center\">HÓA ĐƠN</h1>" +
                $"</br>" +
                $"<h4 style=\"text-align:center\">MÃ ĐƠN HÀNG: #{dh.MaDh}</h4>" +
                $"<h4>Ngày đặt: {dh.NgayDat}</h4>" +
                $"<h4>Hình thức thanh toán: {dh.PhuongThucThanhToan}</h4>" +
                $"<h4>Khách hàng: {dh.HoTen} - {dh.Sdt}</h4>" +
                $"<h4>Địa chỉ giao hàng: {fullAddress}</h4>" +
                $"<table style=\"border-collapse: collapse\" border=\"1\">" +
                $"<tr>" +
                    $"<th width=\"100px\">STT</th>" +
                    $"<th width=\"400px\">Sản phẩm</th>" +
                    $"<th width=\"100px\">Số lượng</th>" +
                    $"<th width=\"100px\">Đơn giá</th>" +
                    $"<th width=\"100px\" align=\"right\">Thành tiền</th>" +
                 $"</tr>";

            string html2 = "";
            int i = 1;
            
            foreach (var item in ctdh)
            {
                html2 +=
                    $"<tr>" +
                        $"<td>{i}</td>" +
                        $"<td>{item.MaSpNavigation.TenSp}</td>" +
                        $"<td>{item.SoLuong}</td>" +
                        $"<td>{item.GiaGiam}</td>" +
                        $"<td align=\"right\">{item.TongTien}</td>" +
                     $"</tr>";
                i++;
            }

            string html3 = "";
            html3 +=
                "<tr>" +
                    "<td align=\"right\" colspan=\"4\">Tạm tính</td>" +
                    $"<td align=\"right\">{ctdh.Sum(x=>x.TongTien)}</td>" +
                "</tr>" +
                "<tr>" +
                    "<td align=\"right\" colspan=\"4\">Giảm giá</td>" +
                    $"<td align=\"right\">{dh.GiamGia}</td>" +
                "</tr>" +
                "<tr>" +
                    "<td align=\"right\" colspan=\"4\">Phí giao hàng</td>" +
                   $" <td align=\"right\">{dh.TienShip-dh.GiamGiaShip}</td>" +
                "</tr>" +
                "<tr>" +
                    "<th align=\"right\" colspan=\"4\">Tổng</th>" +
                    $"<th align=\"right\">{dh.TongTien}</th>" +
                "</tr>" +
                $"</table>" +
                $"" +
                $"</body></html>";

            string htmlContent = html1 + html2 + html3;

            // Convert HTML to PDF
            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                PaperSize = PaperKind.A4,
                Orientation = Orientation.Portrait
            },
                Objects = {
                new ObjectSettings() {
                    HtmlContent = htmlContent,
                    WebSettings = { DefaultEncoding = "utf-8" }
                }
            }
            };

            var pdfBytes = _pdfConverter.Convert(doc);

            // Set the response content type and headers
            Response.ContentType = "application/pdf";
            Response.Headers.Add("content-disposition", $"attachment;filename=HoaDon-DH{dh.MaDh}-{DateTime.Now}.pdf");

            // Write the PDF to the response
            return File(pdfBytes, "application/pdf");
        }




        // GET: Admin/DonHangs/Create
        public IActionResult Create()
        {
            ViewData["MaKh"] = new SelectList(_context.KhachHangs, "MaKh", "MaKh");
            ViewData["MaShipper"] = new SelectList(_context.Shippers, "MaShipper", "MaShipper");
            return View();
        }

        // POST: Admin/DonHangs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaDh,NgayDat,NgayShip,TienShip,GiamGiaShip,GiamGia,DiaChi,TrangThai,MaKh,MaShipper")] DonHang donHang)
        {
            if (ModelState.IsValid)
            {
                _context.Add(donHang);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaKh"] = new SelectList(_context.KhachHangs, "MaKh", "MaKh", donHang.MaKh);
            ViewData["MaShipper"] = new SelectList(_context.Shippers, "MaShipper", "MaShipper", donHang.MaShipper);
            return View(donHang);
        }




        // GET: Admin/DonHangs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var donHang = await _context.DonHangs.FindAsync(id);
            if (donHang == null)
            {
                return NotFound();
            }
            ViewData["MaKh"] = new SelectList(_context.KhachHangs, "MaKh", "MaKh", donHang.MaKh);
            ViewData["MaShipper"] = new SelectList(_context.Shippers, "MaShipper", "MaShipper", donHang.MaShipper);
            return View(donHang);
        }


        // POST: Admin/DonHangs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaDh,NgayDat,NgayShip,TienShip,GiamGiaShip,GiamGia,DiaChi,TrangThai,MaKh,MaShipper")] DonHang donHang)
        {
            if (id != donHang.MaDh)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(donHang);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DonHangExists(donHang.MaDh))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaKh"] = new SelectList(_context.KhachHangs, "MaKh", "MaKh", donHang.MaKh);
            ViewData["MaShipper"] = new SelectList(_context.Shippers, "MaShipper", "MaShipper", donHang.MaShipper);
            return View(donHang);
        }

        // GET: Admin/DonHangs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var donHang = await _context.DonHangs
                .Include(d => d.MaKhNavigation)
                .Include(d => d.MaShipperNavigation)
                .FirstOrDefaultAsync(m => m.MaDh == id);
            if (donHang == null)
            {
                return NotFound();
            }

            return View(donHang);
        }

        // POST: Admin/DonHangs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var donHang = await _context.DonHangs.FindAsync(id);
            _context.DonHangs.Remove(donHang);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DonHangExists(int id)
        {
            return _context.DonHangs.Any(e => e.MaDh == id);
        }
    }
}

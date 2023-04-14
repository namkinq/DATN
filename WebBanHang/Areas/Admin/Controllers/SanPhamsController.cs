using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using WebBanHang.Models;

namespace WebBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SanPhamsController : Controller
    {
        private readonly dbBanHangContext _context;

        public SanPhamsController(dbBanHangContext context)
        {
            _context = context;
        }

        // GET: Admin/SanPhams
        public async Task<IActionResult> Index(int page = 1, int MaLoai = 0)
        {
            List<SanPham> lsSanPham = new List<SanPham>();

            //filter select
            List<SelectListItem> lsQuantityStt = new List<SelectListItem>();
            lsQuantityStt.Add(new SelectListItem() { Text = "Còn hàng", Value = "1" });
            lsQuantityStt.Add(new SelectListItem() { Text = "Hết hàng", Value = "0" });
            ViewData["lsQuantityStt"] = lsQuantityStt;

            ViewData["LoaiSP"] = new SelectList(_context.LoaiSanPhams, "MaLoai", "TenLoai");
            ViewData["ThuongHieu"] = new SelectList(_context.ThuongHieus, "MaTh", "TenTh");

            //filter op
            if(MaLoai != 0)
            {
                lsSanPham = _context.SanPhams
                .AsNoTracking()
                .Where(x=>x.MaLoai== MaLoai)
                .Include(s => s.MaLoaiNavigation)
                .Include(s => s.MaThNavigation)
                .OrderByDescending(x => x.MaSp).ToList();
            }
            else
            {
                lsSanPham = _context.SanPhams
                .AsNoTracking()
                .Include(s => s.MaLoaiNavigation)
                .Include(s => s.MaThNavigation)
                .OrderByDescending(x => x.MaSp).ToList();
            }


            //page
            var pageNumber = page;
            var pageSize = 20;
            PagedList<SanPham> models = new PagedList<SanPham>(lsSanPham.AsQueryable(), pageNumber, pageSize);

            ViewBag.CurrentPage = pageNumber;
            ViewBag.CurrentMaLoai = MaLoai;

            return View(models);
        }

        //
        // Filter(int maLoai=0, int maTh=0, int stt=-1)
        public IActionResult Filter(int MaLoai = 0)
        {
            var url = $"/Admin/SanPhams?MaLoai={MaLoai}";
            if (MaLoai == 0)
            {
                url = $"/Admin/SanPhams";
            }
            else
            {
                //if(maLoai==0) url = $"/Admin/SanPhams?maTh={maTh}&stt={stt}";
            }
            return Json(new { status = "success", RedirectUrl = url });
        }

        // GET: Admin/SanPhams/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPhams
                .Include(s => s.MaLoaiNavigation)
                .Include(s => s.MaThNavigation)
                .FirstOrDefaultAsync(m => m.MaSp == id);
            if (sanPham == null)
            {
                return NotFound();
            }

            return View(sanPham);
        }

        // GET: Admin/SanPhams/Create
        public IActionResult Create()
        {
            ViewData["LoaiSP"] = new SelectList(_context.LoaiSanPhams, "MaLoai", "TenLoai");
            ViewData["ThuongHieu"] = new SelectList(_context.ThuongHieus, "MaTh", "TenTh");
            return View();
        }

        // POST: Admin/SanPhams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaSp,TenSp,GiaBan,GiaGiam,SoLuongCo,Anh,CongSuat,KhoiLuong,MoTa,BaoHanh,MaLoai,MaTh")] SanPham sanPham)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sanPham);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["LoaiSP"] = new SelectList(_context.LoaiSanPhams, "MaLoai", "TenLoai", sanPham.MaLoai);
            ViewData["ThuongHieu"] = new SelectList(_context.ThuongHieus, "MaTh", "TenTh", sanPham.MaTh);
            return View(sanPham);
        }

        // GET: Admin/SanPhams/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham == null)
            {
                return NotFound();
            }
            ViewData["LoaiSP"] = new SelectList(_context.LoaiSanPhams, "MaLoai", "TenLoai", sanPham.MaLoai);
            ViewData["ThuongHieu"] = new SelectList(_context.ThuongHieus, "MaTh", "TenTh", sanPham.MaTh);
            return View(sanPham);
        }

        // POST: Admin/SanPhams/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaSp,TenSp,GiaBan,GiaGiam,SoLuongCo,Anh,CongSuat,KhoiLuong,MoTa,BaoHanh,MaLoai,MaTh")] SanPham sanPham)
        {
            if (id != sanPham.MaSp)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sanPham);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SanPhamExists(sanPham.MaSp))
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
            ViewData["LoaiSP"] = new SelectList(_context.LoaiSanPhams, "MaLoai", "TenLoai", sanPham.MaLoai);
            ViewData["ThuongHieu"] = new SelectList(_context.ThuongHieus, "MaTh", "TenTh", sanPham.MaTh);
            return View(sanPham);
        }

        // GET: Admin/SanPhams/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPhams
                .Include(s => s.MaLoaiNavigation)
                .Include(s => s.MaThNavigation)
                .FirstOrDefaultAsync(m => m.MaSp == id);
            if (sanPham == null)
            {
                return NotFound();
            }

            return View(sanPham);
        }

        // POST: Admin/SanPhams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sanPham = await _context.SanPhams.FindAsync(id);
            _context.SanPhams.Remove(sanPham);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SanPhamExists(int id)
        {
            return _context.SanPhams.Any(e => e.MaSp == id);
        }
    }
}

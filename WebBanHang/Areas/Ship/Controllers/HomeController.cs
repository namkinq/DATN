using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using System.Linq;
using System.Threading.Tasks;
using WebBanHang.Models;

namespace WebBanHang.Areas.Ship.Controllers
{
    [Area("Ship")]
    public class HomeController : Controller
    {
        private readonly dbBanHangContext _context;

        public INotyfService _notyfService { get; }

        public HomeController(dbBanHangContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;

        }
        public async Task<IActionResult> Index(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 10;

            var DonHangs = _context.DonHangs
                .Include(d => d.MaKhNavigation)
                .Include(d => d.MaShipperNavigation)
                .Include(s => s.ChiTietDonHangs)
                .Include(x=>x.MaTtNavigation)
                .AsNoTracking()
                .OrderByDescending(x => x.NgayDat);


            //page
            PagedList<DonHang> models = new PagedList<DonHang>(DonHangs.AsQueryable(), pageNumber, pageSize);

            ViewBag.CurrentPage = pageNumber;

            return View(models);
        }
    }
}

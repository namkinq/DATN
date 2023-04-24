using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using WebBanHang.Models;

namespace WebBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ThongKeController : Controller
    {
        private readonly dbBanHangContext _context;

        public INotyfService _notyfService { get; }

        public ThongKeController(dbBanHangContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;

        }
        public IActionResult Index()
        {
            var salesData = _context.DonHangs.GroupBy(s => s.NgayDat.Value.Month)
                                .Select(g => new { date = g.Key, totalSales = g.Sum(s => s.TongTien) })
                                .ToList();

            ViewBag.SalesData = salesData;

            ViewBag.Data = "10,10,50,20";
            ViewBag.ObjectName = "'Test','Test1','Test2','Test3'";
            return View();
        }
    }
}

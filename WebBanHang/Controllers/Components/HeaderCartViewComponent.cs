﻿using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using WebBanHang.Extension;
using WebBanHang.ModelViews;

namespace WebBanHang.Controllers.Components
{
    public class HeaderCartViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            return View(cart);
        }
    }
}

using Application.DTOs.CartDTOs;
using Application.UnitOfWorks;
using Microsoft.AspNetCore.Mvc;

namespace UIStoreMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CartController : Controller
    {
        private readonly IServiceUnit _services;

        public CartController(IServiceUnit services)
        {
            _services = services;
        }

        // 📌 Sadece görüntüleme: 24 saatten eski sepetler
        [HttpGet]
        public async Task<IActionResult> Abandoned()
        {
            var list = await _services.ShoppingCartService
                                      .GetAbandonedCartsAsync(TimeSpan.Zero);

            return View(list);
        }

        // 📌 Butonla mail gönderip "bir daha listede gösterme"
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendReminder()
        {
            var carts = await _services.ShoppingCartService
                                       .GetAbandonedCartsAsync(TimeSpan.Zero,
                                                               markAsNotified: true);

            // BURADA: gerçek mail gönderimini yaparsın
            // foreach (var c in carts) await _emailSender.SendAbandonedCartMail(c);

            TempData["Success"] = $"{carts.Count} sepete hatırlatma maili gönderildi (varsayım).";
            return RedirectToAction(nameof(Abandoned));
        }

    }
}

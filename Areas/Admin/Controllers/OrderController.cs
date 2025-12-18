using Application.UnitOfWorks;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace UIStoreMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly IServiceUnit _services;

        public OrderController(IServiceUnit services)
        {
            _services = services;
        }

        // 📌 SİPARİŞ LİSTELEME
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var orders = await _services.OrderService.GetAllAsync();
            return View(orders);
        }

        // 📌 SİPARİŞ DETAY
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var order = await _services.OrderService.GetByIdAsync(id); // DTO + OrderItems dahil olmalı

            if (order == null)
            {
                TempData["Error"] = "Sipariş bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            return View(order);
        }

        // 📌 DURUM GÜNCELLE (AJAX)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int orderId, int status)
        {
            try
            {
                var newStatus = (OrderStatus)status;
                await _services.OrderService.UpdateStatusAsync(orderId, newStatus);

                TempData["Success"] = "Sipariş durumu güncellendi.";
                return RedirectToAction(nameof(Index)); //listeye geri dönn
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}

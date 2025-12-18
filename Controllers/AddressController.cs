using System.Security.Claims;
using Application.DTOs.AdressDTOs;
using Application.UnitOfWorks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UIStoreMVC.Controllers
{
    [Authorize]
    public class AddressController : Controller
    {
        private readonly IServiceUnit _services;

        public AddressController(IServiceUnit services)
        {
            _services = services;
        }

        private int GetCurrentUserId()
        {
            var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(idStr!);
        }

        // İstersen adresleri listelemek için kullanırsın
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            var addresses = await _services.AddressService.GetAllByCustomerIdAsync(userId);
            return View(addresses); // View'ı sonra yapabiliriz, şimdilik önemli olan Create
        }

        // GET: /Address/Create
        [HttpGet]
        public IActionResult Create()
        {
            var userId = GetCurrentUserId();

            var model = new CreateAddressDto
            {
                UserId = userId
            };

            return View(model);
        }

        // POST: /Address/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAddressDto model)
        {
            var userId = GetCurrentUserId();
            model = model with { UserId = userId };   // her ihtimale karşı userId’yi burada set ediyoruz

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _services.AddressService.AddAsync(model);

            // Adres eklendikten sonra tekrar Checkout ekranına dön
            return RedirectToAction("Checkout", "Order");
        }
    }
}

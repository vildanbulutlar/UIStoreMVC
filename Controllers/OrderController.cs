using System.Linq;
using System.Security.Claims;
using Application.DTOs.OrderDTOs;
using Application.UnitOfWorks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UIStoreMVC.Models.Checkout;

namespace UIStoreMVC.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IServiceUnit _services;

        public OrderController(IServiceUnit services)
        {
            _services = services;
        }

        private int GetCurrentUserId()
        {
            var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(idStr!);
        }

        // GET: /Order/Checkout
        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var customerId = GetCurrentUserId();

            var vm = await BuildCheckoutViewModel(customerId, null);

            if (vm.Cart == null || vm.Cart.Items == null || !vm.Cart.Items.Any())
            {
                TempData["Warning"] = "Checkout yapmadan önce sepetinize ürün eklemelisiniz.";
                return RedirectToAction("Index", "ShoppingCart");
            }

            return View(vm);
        }


        // POST: /Order/Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutFormModel form)
        {
            var customerId = GetCurrentUserId();

            // 📍 Adres seçilmiş mi?
            if (form.SelectedAddressId == null)
            {
                ModelState.AddModelError(nameof(form.SelectedAddressId),
                    "Lütfen bir teslimat adresi seçiniz.");
            }

            // 💳 Basit ödeme kontrolleri
            if (string.IsNullOrWhiteSpace(form.CardHolderName) ||
                string.IsNullOrWhiteSpace(form.CardNumber) ||
                string.IsNullOrWhiteSpace(form.ExpiryMonth) ||
                string.IsNullOrWhiteSpace(form.ExpiryYear) ||
                string.IsNullOrWhiteSpace(form.Cvv))
            {
                ModelState.AddModelError("", "Lütfen kart bilgilerinizi eksiksiz doldurunuz.");
            }

            if (!ModelState.IsValid)
            {
                // Hatalı durumda ekranı yeniden doldur
                var vmError = await BuildCheckoutViewModel(customerId, form.SelectedAddressId);
                vmError.CardHolderName = form.CardHolderName;
                vmError.CardNumber = form.CardNumber;
                vmError.ExpiryMonth = form.ExpiryMonth;
                vmError.ExpiryYear = form.ExpiryYear;
                vmError.Cvv = form.Cvv;

                return View(vmError);
            }

            // 📌 En güncel sepeti tekrar çek
            var cartItemsLatest = await _services.ShoppingCartService
                .GetCartItemDtoAsync(customerId);

            if (cartItemsLatest == null || !cartItemsLatest.Any())
            {
                TempData["Warning"] = "Sepetiniz boş olduğu için sipariş oluşturulamadı.";
                return RedirectToAction("Index", "ShoppingCart");
            }

            var cartDto = new ShoppingCartDto
            {
                CustomerId = customerId,
                Items = cartItemsLatest.ToList()
            };

            // 📌 Order DTO'sunu hazırla
            var createOrderDto = new CreateOrderDto
            {
                UserId = customerId,
                AddressId = form.SelectedAddressId.Value,
                Items = cartDto.Items.Select(x => new CreateOrderItemDto
                {
                    ProductId = x.ProductId,
                    Quantity = x.Quantity,
                    Price = x.Price
                }).ToList()
                // TotalPrice, CreateOrderDto içinde Items üzerinden hesaplanıyor
            };

            // 📌 Siparişi kaydet
            var orderId = await _services.OrderService.CreateOrderAsync(createOrderDto);

            // 📌 Siparişten sonra sepeti temizle
            await _services.ShoppingCartService.ClearCartAsync(customerId);

            // 📌 Success ekranına git
            return RedirectToAction("Success", new { id = orderId });
        }

        // GET: /Order/Success
        [HttpGet]
        public IActionResult Success(int id)
        {
            ViewBag.OrderId = id;
            return View();
        }

        // GET: /Order/MyOrders
        [HttpGet]
        public async Task<IActionResult> MyOrders()
        {
            var customerId = GetCurrentUserId();
            var orders = await _services.OrderService.GetMyOrdersAsync(customerId);
            return View(orders);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var customerId = GetCurrentUserId();

            var order = await _services.OrderService.GetOrderDetailAsync(id, customerId);

            if (order == null)
            {
                // Bu sipariş yoksa veya başka kullanıcıya aitse
                return NotFound();
            }

            return View(order);
        }

        // Ortak ViewModel hazırlama helper'ı
        private async Task<CheckoutViewModel> BuildCheckoutViewModel(int customerId, int? selectedAddressId)
        {
            // Kullanıcı bilgisi
            var user = await _services.CustomerService.GetByIdAsync(customerId);

            // Adresler
            var addressesEnumerable = await _services.AddressService
                .GetAllByCustomerIdAsync(customerId);

            var addresses = addressesEnumerable.ToList();

            // Default adres seçimi
            if (selectedAddressId == null)
            {
                if (user.DefaultAddressId.HasValue &&
                    addresses.Any(a => a.Id == user.DefaultAddressId.Value))
                {
                    selectedAddressId = user.DefaultAddressId.Value;
                }
                else
                {
                    selectedAddressId = addresses.FirstOrDefault()?.Id;
                }
            }

            // Sepet satırları
            var cartItems = await _services.ShoppingCartService
                .GetCartItemDtoAsync(customerId);

            var cart = new ShoppingCartDto
            {
                CustomerId = customerId,
                Items = cartItems.ToList()
            };

            // ViewModel'i doldur
            return new CheckoutViewModel
            {
                User = user,
                Addresses = addresses,
                SelectedAddressId = selectedAddressId,
                Cart = cart
            };
        }
    }
}

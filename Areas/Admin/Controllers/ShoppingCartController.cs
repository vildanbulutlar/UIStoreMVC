using System.Security.Claims;
using Application.İnterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UIStoreMVC.Controllers
{
    [Authorize]
    public class ShoppingCartController : Controller
    {
        private readonly IShoppingCartService _shoppingCartService;

        public ShoppingCartController(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }

        private int GetCurrentUserId()
        {
            var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(idStr!);
        }

        // GET: /ShoppingCart
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();

            var cartItems = await _shoppingCartService.GetCartItemDtoAsync(userId);

            // Model: IEnumerable<CartItemDto>
            return View(cartItems);
        }

        // POST: /ShoppingCart/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int productId, int quantity = 1, string? returnUrl = null)
        {
            var userId = GetCurrentUserId();

            await _shoppingCartService.AddItemAsync(userId, productId, quantity);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction(nameof(Index));
        }

        // POST: /ShoppingCart/Remove
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int productId)
        {
            var userId = GetCurrentUserId();

            await _shoppingCartService.RemoveItemAsync(userId, productId);

            return RedirectToAction(nameof(Index));
        }

        // POST: /ShoppingCart/UpdateQuantity
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(int productId, int quantity)
        {
            var userId = GetCurrentUserId();

            await _shoppingCartService.UpdateQuantityAsync(userId, productId, quantity);

            return RedirectToAction(nameof(Index));
        }

        // POST: /ShoppingCart/Clear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Clear()
        {
            var userId = GetCurrentUserId();

            await _shoppingCartService.ClearCartAsync(userId);

            return RedirectToAction(nameof(Index));
        }
    }
}

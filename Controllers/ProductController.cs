using Application.DTOs.ProductDTOs;
using Application.UnitOfWorks;
using Microsoft.AspNetCore.Mvc;
using UIStoreMVC.Models;

namespace UIStoreMVC.Controllers
{
    public class ProductController : Controller
    {
        private readonly IServiceUnit _services;

        public ProductController(IServiceUnit services)
        {
            _services = services;
        }

        // ÜRÜN DETAY SAYFASI: /Product/Detail/1
        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var product = await _services.ProductService.GetByIdWithDetailsAsync(id);
            return product == null ? NotFound() : View(product);
        }
        [HttpGet]
        public async Task<IActionResult> DetailByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return RedirectToAction(nameof(List));

            var decoded = Uri.UnescapeDataString(name).Trim();

            // sadece id bulmak için minimum iş:
            var all = await _services.ProductService.GetAllAsync();
            var match = all.FirstOrDefault(p =>
                !string.IsNullOrWhiteSpace(p.Name) &&
                string.Equals(p.Name.Trim(), decoded, StringComparison.OrdinalIgnoreCase));

            if (match == null)
                return RedirectToAction(nameof(List), new { SearchTerm = decoded });

            return RedirectToAction(nameof(Detail), new { id = match.Id });
        }


        // /Product/List?CategoryId=1&ChipFilter=discounted&SearchTerm=kalem&MinPrice=10&MaxPrice=50&SortBy=price_asc
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] ProductFilterViewModel filters)
        {
            IEnumerable<ProductDto> products;

            if (filters.CategoryId.HasValue)
            {
                products = await _services.ProductService.GetByCategoryIdAsync(filters.CategoryId.Value);
                ViewData["SelectedCategoryId"] = filters.CategoryId;
            }
            else
            {
                products = await _services.ProductService.GetAllAsync();
                ViewData["SelectedCategoryId"] = null;
            }

            // Chip Filter
            var chip = filters.ChipFilter?.ToLowerInvariant();
            ViewData["Filter"] = chip;

            products = chip switch
            {
                "discounted" => products.Where(p => p.Discount < 1m),

                "today" or "new" => products
                    .OrderByDescending(p => p.Id)
                    .Take(12),

                "topseller" => products
                    .OrderByDescending(p => p.IsFeatured)
                    .ThenBy(p => p.Price)
                    .Take(12),

                "stock" => products.Where(p => p.Stock > 0), // ✅ view'da var diye eklendi

                _ => products
            };

            // Search
            if (!string.IsNullOrWhiteSpace(filters.SearchTerm))
            {
                products = products.Where(p =>
                    p.Name != null &&
                    p.Name.Contains(filters.SearchTerm, StringComparison.OrdinalIgnoreCase));
            }

            // Price Range
            if (filters.MinPrice.HasValue)
                products = products.Where(p => p.Price >= filters.MinPrice.Value);

            if (filters.MaxPrice.HasValue)
                products = products.Where(p => p.Price <= filters.MaxPrice.Value);

            // Checkboxes
            if (filters.OnlyDiscounted)
                products = products.Where(p => p.Discount < 1m);

            if (filters.InStockOnly)
                products = products.Where(p => p.Stock > 0);

            // Sort
            products = filters.SortBy switch
            {
                "price_asc" => products.OrderBy(p => p.Price),
                "price_desc" => products.OrderByDescending(p => p.Price),
                "name_asc" => products.OrderBy(p => p.Name),
                "name_desc" => products.OrderByDescending(p => p.Name),
                _ => products
            };

            filters.Products = products.ToList();
            return View(filters);
        }
    }
}

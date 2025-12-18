using Application.UnitOfWorks;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace UIStoreMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IServiceUnit _services;

        public HomeController(IServiceUnit services)
        {
            _services = services;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // DB'den gelen veriyi ekrana basacağız, DB boşsa da "boş model" döneceğiz.
            var model = new HomeViewModel();

            var categoriesDto = await _services.CategoryService.GetAllAsync();
            var productsDto = await _services.ProductService.GetAllAsync();

            // KATEGORİLER
            if (categoriesDto != null && categoriesDto.Any())
            {
                model.Categories = categoriesDto.Select(c => new CategoryVm
                {
                    Id = c.Id,
                    Name = c.Name,
                    Icon = string.IsNullOrWhiteSpace(c.Icon) ? "📦" : c.Icon
                }).ToList();
            }

            // ÜRÜNLER (anasayfa için 12 tane gösterelim)
            if (productsDto != null && productsDto.Any())
            {
                model.Products = productsDto
                    .OrderByDescending(p => p.IsFeatured)
                    .ThenByDescending(p => p.Id)
                    .Take(12)
                    .Select(p => new ProductVm
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Price = p.Price,
                        Stock = p.Stock,
                        CategoryId = p.CategoryId,
                        Description = p.Description ?? "",
                        // Şimdilik tek resim: ImageUrl (Versiyon B'ye geçince CoverImageUrl olacak)
                        ImageUrl = string.IsNullOrWhiteSpace(p.ImageUrl) ? "/images/no-image.jpg" : p.ImageUrl
                    })
                    .ToList();
            }

            return View(model);
        }
    }

    // ===== VIEWMODEL (aynı dosyada kalsın dedin diye burada) =====
    public class HomeViewModel
    {
        public List<CategoryVm> Categories { get; set; } = new();
        public List<ProductVm> Products { get; set; } = new();

        public bool IsEmpty => (Categories == null || Categories.Count == 0)
                            && (Products == null || Products.Count == 0);
    }

    public class CategoryVm
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Icon { get; set; } = "📦";
    }

    public class ProductVm
    {
        public int Id { get; set; }                // ileride detay sayfasına gideceksin diye ekledim
        public string Name { get; set; } = "";
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int CategoryId { get; set; }
        public string Description { get; set; } = "";
        public string ImageUrl { get; set; } = "/images/no-image.jpg";
    }
}

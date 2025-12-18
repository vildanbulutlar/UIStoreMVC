using Application.DTOs.ProductDTOs;
using Application.UnitOfWorks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace UIStoreMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IServiceUnit _services;
        private readonly IWebHostEnvironment _env;

        public ProductController(IServiceUnit services, IWebHostEnvironment env)
        {
            _services = services;
            _env = env;
        }

        // YARDIMCI: Kategori dropdown için
        private async Task LoadCategoriesAsync(int? selectedId = null)
        {
            var categories = await _services.CategoryService.GetAllAsync();

            ViewBag.Categories = categories
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name,
                    Selected = selectedId.HasValue && selectedId.Value == c.Id
                })
                .ToList();
        }

        // ✅ Tek noktadan dosya kaydet (Create + Update ortak)
        private async Task<string> SaveImageAsync(IFormFile file)
        {
            var ext = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{ext}";

            var folder = Path.Combine(_env.WebRootPath, "images", "products");
            Directory.CreateDirectory(folder);

            var fullPath = Path.Combine(folder, fileName);

            using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/images/products/{fileName}";
        }

        // GET: /Admin/Product
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var products = await _services.ProductService.GetAllAsync();
            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadCategoriesAsync();
            return View(new CreateProductDto());
        }

        // ✅ DOSYA UPLOAD + CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProductDto model)
        {
            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync(model.CategoryId);
                return View(model);
            }

            try
            {
                if (model.ImageFile != null && model.ImageFile.Length > 0)
                    model.ImageUrl = await SaveImageAsync(model.ImageFile);
                else
                    model.ImageUrl = "/images/no-image.jpg";

                await _services.ProductService.AddAsync(model);

                TempData["SuccessMessage"] = "Ürün başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ürün eklenirken bir hata oluştu: " + ex.Message;
                await LoadCategoriesAsync(model.CategoryId);
                return View(model);
            }
        }

        // GET: /Admin/Product/Update/5
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var product = await _services.ProductService.GetUpdateDtoByIdAsync(id);
            if (product == null)
            {
                TempData["ErrorMessage"] = "Ürün bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            await LoadCategoriesAsync(product.CategoryId);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateProductDto model)
        {
            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync(model.CategoryId);
                return View(model);
            }

            try
            {
                // yeni resim seçildiyse değiştir, seçilmediyse hidden ImageUrl aynen kalsın
                if (model.ImageFile != null && model.ImageFile.Length > 0)
                    model.ImageUrl = await SaveImageAsync(model.ImageFile);

                await _services.ProductService.UpdateAsync(model);

                TempData["SuccessMessage"] = "Ürün başarıyla güncellendi.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ürün güncellenirken hata oluştu: " + ex.Message;
                await LoadCategoriesAsync(model.CategoryId);
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _services.ProductService.SoftDeleteAsync(id);
                TempData["SuccessMessage"] = "Ürün başarıyla silindi.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ürün silinemedi: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Trash()
        {
            var deleted = await _services.ProductService.GetAllDeletedAsync();
            return View(deleted);
        }

        [HttpPost]
        public async Task<IActionResult> Restore(int id)
        {
            await _services.ProductService.RestoreAsync(id);
            return RedirectToAction(nameof(Trash));
        }

        [HttpPost]
        public async Task<IActionResult> HardDelete(int id)
        {
            await _services.ProductService.DeleteAsync(id);
            return RedirectToAction(nameof(Trash));
        }
    }
}

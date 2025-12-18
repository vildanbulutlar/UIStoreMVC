using Application.DTOs.CategoryDTOs;
using Application.UnitOfWorks;
using Microsoft.AspNetCore.Mvc;

namespace UIStoreAppMvc.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IServiceUnit _services;

        public CategoryController(IServiceUnit services)
        {
            _services = services;
        }

        // GET: /Admin/Category/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var categories = await _services.CategoryService.GetAllAsync();
                return View(categories);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Kategoriler yüklenemedi: " + ex.Message;
                return View(new List<CategoryDto>());
            }
        }

        // GET: /Admin/Category/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Admin/Category/Create
        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryDto model)
        {
            // MANUEL VALIDATION EKLENDİ
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _services.CategoryService.AddAsync(model);
                TempData["SuccessMessage"] = "Kategori başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Kategori eklenemedi: " + ex.Message;
                return View(model);
            }
        }

        // GET: /Admin/Category/Update/5
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            try
            {
                var category = await _services.CategoryService.GetByIdAsync(id);
                if (category == null)
                {
                    TempData["ErrorMessage"] = "Kategori bulunamadı!";
                    return RedirectToAction(nameof(Index));
                }

                var updateDto = new UpdateCategoryDto()
                {
                    Id = category.Id,
                    Name = category.Name
                };

                return View(updateDto);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Kategori yüklenemedi: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /Admin/Category/Update
        [HttpPost]
        public async Task<IActionResult> Update(UpdateCategoryDto model)
        {
            // MANUEL VALIDATION EKLENDİ
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _services.CategoryService.UpdateAsync(model);
                TempData["SuccessMessage"] = "Kategori başarıyla güncellendi.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Kategori güncellenemedi: " + ex.Message;
                return View(model);
            }
        }

        // POST: /Admin/Category/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _services.CategoryService.SoftDeleteAsync(id);
                TempData["SuccessMessage"] = "Kategori başarıyla silindi.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Kategori silinemedi: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/Category/Trash
        [HttpGet]
        public async Task<IActionResult> Trash()
        {
            try
            {
                var deletedCategories = await _services.CategoryService.GetAllDeletedAsync();
                return View(deletedCategories);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Silinen kategoriler yüklenemedi: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /Admin/Category/Restore/5
        [HttpPost]
        public async Task<IActionResult> Restore(int id)
        {
            try
            {
                await _services.CategoryService.RestoreAsync(id);
                TempData["SuccessMessage"] = "Kategori başarıyla geri yüklendi.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Kategori geri yüklenemedi: " + ex.Message;
            }

            return RedirectToAction(nameof(Trash));
        }

        // POST: /Admin/Category/HardDelete/5
        [HttpPost]
        public async Task<IActionResult> HardDelete(int id)
        {
            try
            {
                await _services.CategoryService.DeleteAsync(id);
                TempData["SuccessMessage"] = "Kategori kalıcı olarak silindi.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Kategori silinemedi: " + ex.Message;
            }

            return RedirectToAction(nameof(Trash));
        }
    }
}
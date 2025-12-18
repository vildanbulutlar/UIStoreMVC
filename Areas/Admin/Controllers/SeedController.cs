using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace UIStoreMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SeedController : Controller
    {
        private readonly ISeedImportService _import;

        public SeedController(ISeedImportService import)
        {
            _import = import;
        }

        [HttpGet]
        public IActionResult Import()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(IFormFile jsonFile)
        {
            if (jsonFile == null || jsonFile.Length == 0)
            {
                TempData["Error"] = "Lütfen bir JSON dosyası seçin.";
                return RedirectToAction(nameof(Import));
            }

            if (!jsonFile.FileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            {
                TempData["Error"] = "Dosya uzantısı .json olmalı.";
                return RedirectToAction(nameof(Import));
            }

            using var reader = new StreamReader(jsonFile.OpenReadStream());
            var json = await reader.ReadToEndAsync();

            try
            {
                await _import.ImportFromJsonAsync(json);
                TempData["Success"] = "JSON import tamamlandı ✅";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Import sırasında hata: " + ex.Message;
            }

            return RedirectToAction(nameof(Import));
        }
    }
}

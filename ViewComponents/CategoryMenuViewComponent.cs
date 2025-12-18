using Application.DTOs.CategoryDTOs;
using Application.Interfaces; // sende kategori servisi neredeyse orası
using Application.UnitOfWorks;
using Microsoft.AspNetCore.Mvc;

namespace UIStoreMVC.ViewComponents
{
    public class CategoryMenuViewComponent : ViewComponent
    {
        private readonly IServiceUnit _services; // sende servis yapısı buysa
        public CategoryMenuViewComponent(IServiceUnit services)
        {
            _services = services;
        }

        public async Task<IViewComponentResult> InvokeAsync(bool asDropdown = true, int? selectedCategoryId = null)
        {
            var categories = await _services.CategoryService.GetAllAsync(); // sende hangi methodsa

            ViewBag.SelectedCategoryId = selectedCategoryId;

            // ✅ artık “arayacağım view adı belli”
            return asDropdown
                ? View("NavbarMega", categories)
                : View("SidebarList", categories);
        }
    }
}

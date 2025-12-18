using Application.İnterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UIStoreMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AllowAnonymous] // test için; sonra [Authorize(Roles = "Admin")] yaparsın
    public class AgencyApplicationController : Controller
    {
        private readonly IAgencyApplicationService _agencyService;

        public AgencyApplicationController(IAgencyApplicationService agencyService)
        {
            _agencyService = agencyService;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _agencyService.GetPendingApplicationsAsync();
            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> Approve(int id)
        {
            var adminId = 0; // şimdilik sabit

            await _agencyService.ApproveAsync(id, adminId);

            TempData["Success"] = "Ajans başvurusu onaylandı.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Reject(int id)
        {
            var adminId = 0;

            await _agencyService.RejectAsync(id, adminId);

            TempData["Success"] = "Ajans başvurusu reddedildi.";
            return RedirectToAction(nameof(Index));
        }
    }
}

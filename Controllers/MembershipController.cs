using Application.İnterfaces;                 // ICustomerService, IAgencyApplicationService
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UIStoreMVC.Models.Membership;           // ViewModel’ler
using UIStoreMVC.Models.Membership.Agency;    // Ajans viewmodel
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace UIStoreMVC.Controllers
{
    [Authorize]
    public class MembershipController : Controller
    {
        private readonly UserManager<Customer> _userManager;
        private readonly ICustomerService _customerService;
        private readonly IAgencyApplicationService _agencyService;
        private readonly IWebHostEnvironment _env;

        public MembershipController(
            UserManager<Customer> userManager,
            ICustomerService customerService,
            IAgencyApplicationService agencyService,
            IWebHostEnvironment env)
        {
            _userManager = userManager;
            _customerService = customerService;
            _agencyService = agencyService;
            _env = env;
        }

        // ======================
        //  DASHBOARD
        // ======================
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            // 🔹 Kullanıcının varsa ajans başvurusunu çekiyoruz
            var application = await _agencyService.GetMyApplicationAsync(user.Id);

            var vm = new MembershipDashboardViewModel
            {
                FullName = user.FullName,
                Email = user.Email,
                MembershipType = user.MembershipType,
                IsVipActive = user.IsVipActive,
                HasAgencyMembership = user.MembershipType == MembershipType.Agency,

                // 🔹 Ajans başvurusu ile ilgili alanlar
                HasAgencyApplication = application != null,
                AgencyApplicationStatus = application?.ApplicationStatus,
                AgencyApplicationRejectionReason = application?.RejectionReason
            };

            return View(vm);
        }



        // ======================
        //  VIP ÜYELİK
        // ======================

        [HttpGet]
        public IActionResult Vip()
        {
            // Burada normalde fiyat, paket bilgisi, ödeme vb. gösterirsin.
            return View();
        }



        // ======================
        //  AJANS BAŞVURUSU
        // ======================

        [HttpGet]
        public IActionResult AgencyApply()
        {
            var vm = new AgencyApplicationCreateViewModel();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgencyApply(AgencyApplicationCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            // Belgeleri wwwroot/uploads/agency altına kaydedelim
            string uploadRoot = Path.Combine(_env.WebRootPath, "uploads", "agency");
            Directory.CreateDirectory(uploadRoot);

            string SaveFile(IFormFile? file)
            {
                if (file == null || file.Length == 0)
                    return string.Empty;

                var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                var fullPath = Path.Combine(uploadRoot, fileName);
                using (var stream = System.IO.File.Create(fullPath))
                {
                    file.CopyTo(stream);
                }
                // Veritabanında sadece göreli yolu tutuyoruz
                return $"/uploads/agency/{fileName}";
            }

            var taxDocPath = SaveFile(model.TaxDocument);
            var signDocPath = SaveFile(model.SignatureCircular);
            var tradeRegPath = SaveFile(model.TradeRegistry);

            var application = new AgencyApplication
            {
                CustomerId = user.Id,
                CompanyName = model.CompanyName,
                TaxNumber = model.TaxNumber,
                Phone = model.Phone ?? string.Empty,
                ContactPerson = model.ContactPerson ?? string.Empty,
                TaxDocumentPath = taxDocPath,
                SignatureCircularPath = signDocPath,
                TradeRegistryPath = tradeRegPath,
                ApplicationStatus = AgencyApplicationStatus.WaitingForReview, // 🔹 başvuru incelemede
                AppliedDate = DateTime.Now
            };

            await _agencyService.AddAsync(application);

            TempData["MembershipMessage"] = "Ajans başvurunuz alındı. Onaylandıktan sonra ajans üyeliğiniz aktif edilecek.";
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> MyAgencyApplication()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var dto = await _agencyService.GetMyApplicationAsync(user.Id);
            return View(dto);
        }
        [HttpPost]
        public async Task<IActionResult> BuyVip()
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

            await _customerService.UpgradeCustomerToVipAsync(userId);

            TempData["Success"] = "VIP üyelik başarıyla aktif edildi.";

            // ✅ VIP olduktan sonra ana sayfa yerine Üyeliklerim sayfasında kal
            return RedirectToAction(nameof(Index), "Membership");
        }


    }
}

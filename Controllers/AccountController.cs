using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UIStoreMVC.Models;

namespace UIStoreMVC.Controllers
{
    /// <summary>
    /// Kullanıcı kayıt / giriş / çıkış ve e-posta doğrulama işlemlerini yönetir.
    /// </summary>
    [AllowAnonymous] // Bu controller'daki aksiyonlara anonim erişim serbest
    public class AccountController : Controller
    {
        private readonly UserManager<Customer> _userManager;
        private readonly SignInManager<Customer> _signInManager;

        public AccountController(
            UserManager<Customer> userManager,
            SignInManager<Customer> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // ============================
        // REGISTER (ÜYE OL)
        // ============================

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // 1) Herkes önce NORMAL (Standard) müşteri olarak oluşturuluyor
            var customer = new Customer(model.FullName, model.Email, model.Email);

            // 2) Identity üzerinden kullanıcıyı kaydet
            var result = await _userManager.CreateAsync(customer, model.Password);

            if (result.Succeeded)
            {
                // 3) E-posta doğrulama kodu üret
                customer.GenerateEmailVerificationCode();
                await _userManager.UpdateAsync(customer);

                // Demo amaçlı: kodu TempData ile VerifyEmail ekranına gönderiyoruz
                TempData["VerifyCode"] = customer.EmailVerificationCode;

                // 4) Otomatik giriş YOK, önce e-posta doğrulaması yapılacak
                return RedirectToAction("VerifyEmail", new { email = customer.Email });
            }

            // 5) Identity hatalarını ekrana bas
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        // ============================
        // E-POSTA DOĞRULAMA
        // ============================

        [HttpGet]
        public IActionResult VerifyEmail(string email)
        {
            ViewBag.Email = email;
            ViewBag.Code = TempData["VerifyCode"];
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyEmail(string email, string code)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                ModelState.AddModelError("", "Kullanıcı bulunamadı.");
                return View();
            }

            // Kod yanlış ya da süresi dolmuşsa
            if (user.EmailVerificationCode != code ||
                user.EmailVerificationExpireDate < DateTime.Now)
            {
                ModelState.AddModelError("", "Doğrulama kodu hatalı veya süresi dolmuş.");
                ViewBag.Email = email;
                return View();
            }

            // Doğrulama başarılı
            user.IsEmailVerified = true;
            user.EmailVerificationCode = null;
            user.EmailVerificationExpireDate = null;

            await _userManager.UpdateAsync(user);

            TempData["VerifySuccess"] = "E-posta doğrulandı, şimdi giriş yapabilirsiniz.";
            return RedirectToAction("Login");
        }

        // ============================
        // LOGIN (GİRİŞ)
        // ============================

        // 🔹 GET: /Account/Login  --> 405 hatasını çözen kısım burası
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(); // Views/Account/Login.cshtml
        }

        // 🔹 POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
                return View(model);

            // 1) Kullanıcıyı bul
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Kullanıcı bulunamadı.");
                return View(model);
            }

            // 2) Admin değilse email doğrulama zorunlu
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            if (!isAdmin && !user.IsEmailVerified)
            {
                ModelState.AddModelError(string.Empty, "Lütfen önce e-posta adresinizi doğrulayın.");
                return View(model);
            }

            // 3) Giriş denemesi
            var result = await _signInManager.PasswordSignInAsync(
                user,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                // 🔹 Admin ise → Admin/Home/Index
                if (isAdmin)
                {
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }

                // 🔹 Normal kullanıcıysa returnUrl varsa oraya, yoksa Home/Index
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "E-posta veya şifre hatalı.");
            return View(model);
        }

        // ============================
        // LOGOUT (ÇIKIŞ)
        // ============================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}

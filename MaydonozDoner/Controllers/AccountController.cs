using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using MaydonozDoner.Models;

namespace MaydonozDoner.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;

        public AccountController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(model.Username);
                    if (user != null)
                    {
                        var roles = await _userManager.GetRolesAsync(user);
                        if (roles.Contains("Admin"))
                        {
                            return RedirectToAction("Index", "Admin");
                        }
                    }
                    return RedirectToAction("Index", "Product");
                }
                
                var userByEmail = await _userManager.FindByEmailAsync(model.Username);
                if (userByEmail != null)
                {
                    var resultEmail = await _signInManager.PasswordSignInAsync(userByEmail.UserName!, model.Password, model.RememberMe, lockoutOnFailure: false);
                    if (resultEmail.Succeeded)
                    {
                        var roles = await _userManager.GetRolesAsync(userByEmail);
                        if (roles.Contains("Admin"))
                        {
                            return RedirectToAction("Index", "Admin");
                        }
                        return RedirectToAction("Index", "Product");
                    }
                }

                ModelState.AddModelError(string.Empty, "Geçersiz kullanıcı adı veya şifre.");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser 
                { 
                    UserName = model.Username, 
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber
                };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "User");
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Product");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var model = new ProfileViewModel
            {
                Username = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                Address = user.Address,
                CardNumber = user.CardNumber,
                CardHolderName = user.CardHolderName,
                CardExpiry = user.CardExpiry,
                CardCvv = user.CardCvv
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login");
                }

                user.PhoneNumber = model.PhoneNumber;
                user.Address = model.Address ?? string.Empty;
                user.CardNumber = model.CardNumber ?? string.Empty;
                user.CardHolderName = model.CardHolderName ?? string.Empty;
                user.CardExpiry = model.CardExpiry ?? string.Empty;
                user.CardCvv = model.CardCvv ?? string.Empty;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Profil bilgileriniz başarıyla güncellendi.";
                    return RedirectToAction("Profile");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetProfileData()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = false, message = "Oturum açık değil." });
            }

            return Json(new {
                success = true,
                phoneNumber = user.PhoneNumber ?? string.Empty,
                address = user.Address ?? string.Empty,
                cardNumber = user.CardNumber ?? string.Empty,
                cardHolderName = user.CardHolderName ?? string.Empty,
                cardExpiry = user.CardExpiry ?? string.Empty,
                cardCvv = user.CardCvv ?? string.Empty
            });
        }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "Kullanıcı adı veya E-posta zorunludur.")]
        [Display(Name = "Kullanıcı Adı / E-posta")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Beni Hatırla")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
        [Display(Name = "Kullanıcı Adı")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta adresi zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçersiz E-posta adresi.")]
        [Display(Name = "E-posta")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefon numarası zorunludur.")]
        [RegularExpression(@"^[0-9\s\-\(\)\+]+$", ErrorMessage = "Telefon numarası sadece rakam, boşluk, parantez veya tire içerebilir.")]
        [StringLength(30, MinimumLength = 10, ErrorMessage = "Telefon numarası en az 10 karakter olmalıdır.")]
        [Display(Name = "Telefon Numarası")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [StringLength(100, ErrorMessage = "Şifre en az {2} karakter olmalıdır.", MinimumLength = 4)]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Şifre Tekrar")]
        [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class ProfileViewModel
    {
        [Display(Name = "Kullanıcı Adı")]
        public string Username { get; set; } = string.Empty;

        [Display(Name = "E-posta")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefon numarası zorunludur.")]
        [RegularExpression(@"^[0-9\s\-\(\)\+]+$", ErrorMessage = "Telefon numarası sadece rakam, boşluk, parantez veya tire içerebilir.")]
        [StringLength(30, MinimumLength = 10, ErrorMessage = "Telefon numarası en az 10 karakter olmalıdır.")]
        [Display(Name = "Telefon Numarası")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Display(Name = "Adresiniz")]
        [StringLength(500, ErrorMessage = "Adres en fazla 500 karakter olabilir.")]
        public string Address { get; set; } = string.Empty;

        [RegularExpression(@"^$|^[0-9\s\-]{15,25}$", ErrorMessage = "Kart numarası boş bırakılabilir veya geçerli bir kart numarası olmalıdır.")]
        [Display(Name = "Kart Numarası")]
        public string? CardNumber { get; set; } = string.Empty;

        [Display(Name = "Kart Üzerindeki İsim")]
        public string? CardHolderName { get; set; } = string.Empty;

        [RegularExpression(@"^$|^(0[1-9]|1[0-2])\/[0-9]{2}$", ErrorMessage = "Geçersiz format. Örn: 08/29")]
        [Display(Name = "Son Kullanma Tarihi (AA/YY)")]
        public string? CardExpiry { get; set; } = string.Empty;

        [RegularExpression(@"^$|^[0-9]{3,4}$", ErrorMessage = "CVV boş bırakılabilir veya 3-4 haneli rakam olmalıdır.")]
        [Display(Name = "CVV Güvenlik Kodu")]
        public string? CardCvv { get; set; } = string.Empty;
    }
}

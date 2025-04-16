using System.Security.Claims;
using System.Threading.Tasks;
using GeziBlogum.Data.Abstract;
using GeziBlogum.Entity;
using GeziBlogum.Models;
using GeziBlogum.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GeziBlogum.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Index", "Posts");
            }
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["Message"] = "\u00c7\u0131k\u0131\u015f yap\u0131ld\u0131.";
            return RedirectToAction("Login");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userRepository.Users.FirstOrDefaultAsync(x => x.UserName == model.Username || x.Email == model.Email);
                if (user == null)
                {
                    _userRepository.CreateUser(new User
                    {
                        UserName = model.Username,
                        Name = model.Name,
                        Email = model.Email,
                        Password = model.Password,
                        Image = "p1.jpg"
                    });

                    TempData["Message"] = "Kay\u0131t ba\u015far\u0131l\u0131! Giri\u015f yapabilirsiniz.";
                    return RedirectToAction("Login");
                }
                else
                {
                    ModelState.AddModelError("", "Username ya da Email kullan\u0131mda.");
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var isUser = await _userRepository.Users.FirstOrDefaultAsync(x => x.Email == model.Email && x.Password == model.Password);

                if (isUser != null)
                {
                    var userClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, isUser.UserId.ToString()),
                        new Claim(ClaimTypes.Name, isUser.UserName ?? ""),
                        new Claim(ClaimTypes.GivenName, isUser.Name ?? ""),
                        new Claim(ClaimTypes.UserData, isUser.Image ?? "")
                    };

                    if (isUser.Email == "info@alperencocalak.com")
                    {
                        userClaims.Add(new Claim(ClaimTypes.Role, "admin"));
                    }

                    var claimsIdentity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties { IsPersistent = true };

                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties
                    );

                    TempData["Message"] = "Ba\u015far\u0131yla giri\u015f yapt\u0131n\u0131z.";
                    return RedirectToAction("Index", "Posts");
                }
                else
                {
                    ModelState.AddModelError("", "Kullan\u0131c\u0131 ad\u0131 veya \u015fifre hatal\u0131.");
                }
            }
            return View(model);
        }

        public IActionResult Profile(string username)
        {
            if (string.IsNullOrEmpty(username)) return NotFound();

            var user = _userRepository.Users
                .Include(x => x.Posts)
                .Include(x => x.Comments)
                .ThenInclude(x => x.Post)
                .FirstOrDefault(x => x.UserName == username);

            if (user == null) return NotFound();

            return View(user);
        }


        [Authorize]
        [HttpGet]
        public IActionResult EditProfile()
        {
            var username = User.Identity?.Name;
            var user = _userRepository.Users.FirstOrDefault(x => x.UserName == username);

            if (user == null)
            {
                return NotFound();
            }

            var model = new ProfileUpdateViewModel
            {
                UserId = user.UserId,
                Name = user.Name,
                Username = user.UserName,
                Image = user.Image
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]

        public async Task<IActionResult> EditProfile(ProfileUpdateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Message"] = "Eksik veya hatalı bilgi girdiniz.";
                return RedirectToAction("Profile", new { username = User.Identity?.Name });
            }

            var user = await _userRepository.Users.FirstOrDefaultAsync(x => x.UserId == model.UserId);
            if (user == null) return NotFound();


            if (!string.IsNullOrEmpty(model.CurrentPassword) || !string.IsNullOrEmpty(model.NewPassword))
            {
                if (model.CurrentPassword != user.Password)
                {
                    TempData["Message"] = "Mevcut şifre yanlış.";
                    return RedirectToAction("Profile", new { username = User.Identity?.Name });
                }

                if (model.NewPassword != model.ConfirmPassword)
                {
                    TempData["Message"] = "Yeni şifreler uyuşmuyor.";
                    return RedirectToAction("Profile", new { username = User.Identity?.Name });
                }

                user.Password = model.NewPassword;
            }

            if (model.ImageFile != null)
            {
                user.Image = await FileHelper.FileLoaderAsync(model.ImageFile);
            }

            _userRepository.UpdateUser(user);

            TempData["Message"] = "Profil başarıyla güncellendi.";
            return RedirectToAction("Profile", new { username = user.UserName });
        }

    }
}
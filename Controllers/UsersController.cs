using System.Security.Claims;
using System.Threading.Tasks;
using GeziBlogum.Data.Abstract;
using GeziBlogum.Data.Concrete.EfCore;
using GeziBlogum.Entity;
using GeziBlogum.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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
            TempData["Message"] = "Çıkış yapıldı.";
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
                    _userRepository.CreateUser(new Entity.User
                    {
                        UserName = model.Username,
                        Name = model.Name,
                        Email = model.Email,
                        Password = model.Password,
                        Image = "p1.jpg"
                    });

                    TempData["Message"] = "Kayıt başarılı! Giriş yapabilirsiniz.";
                    return RedirectToAction("Login");
                }
                else
                {
                    ModelState.AddModelError("", "Username ya da Email kullanımda.");
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

                    TempData["Message"] = "Başarıyla giriş yaptınız.";
                    return RedirectToAction("Index", "Posts");
                }
                else
                {
                    ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı.");
                }
            }
            return View(model);
        }

        public IActionResult Profile(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return NotFound();
            }

            var user = _userRepository.Users
                .Include(x => x.Posts)
                .Include(x => x.Comments)
                .ThenInclude(x => x.Post)
                .FirstOrDefault(x => x.UserName == username);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
    }
}

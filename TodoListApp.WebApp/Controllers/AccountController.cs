using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.Services.WebApi.Services;
using TodoListApp.WebApi.Models.Auth;

namespace TodoListApp.WebApp.Controllers;
public class AccountController : Controller
{
    private readonly AuthWebApiService authService;

    public AccountController(AuthWebApiService authService)
    {
        this.authService = authService;
    }

    public IActionResult Index()
    {
        return this.View();
    }

    [HttpGet]
    public IActionResult Login()
    {
        return this.View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginModel model)
    {
        if (this.ModelState.IsValid)
        {
            var result = await this.authService.LoginAsync(model);
            if (result.Success)
            {
                await this.SignInUser(result.Token);
                Console.WriteLine(result.Token);
                return this.RedirectToAction("Index", "Home");
            }

            this.ModelState.AddModelError(string.Empty, result.Message);
        }

        return this.View(model);
    }

    [HttpGet]
    public IActionResult Register()
    {
        return this.View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterModel model)
    {
        if (this.ModelState.IsValid)
        {
            var result = await this.authService.RegisterAsync(model);
            if (result.Success)
            {
                await this.SignInUser(result.Token);
                return this.RedirectToAction("Index", "TodoList");
            }

            this.ModelState.AddModelError(string.Empty, result.Message);
        }

        return this.View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await this.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return this.RedirectToAction("Index", "Home");
    }

    private async Task SignInUser(string token)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, token),
        };

        var claimsIdentity = new ClaimsIdentity(
            claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7),
        };

        await this.HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);
    }
}

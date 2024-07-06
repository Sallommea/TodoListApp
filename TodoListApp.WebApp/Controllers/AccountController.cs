using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.Services.WebApi.Services;
using TodoListApp.WebApi.Models.Auth;
using TodoListApp.WebApp.Logging;

namespace TodoListApp.WebApp.Controllers;
public class AccountController : Controller
{
    private readonly AuthWebApiService authService;
    private readonly ILogger<AccountController> logger;

    public AccountController(AuthWebApiService authService, ILogger<AccountController> logger)
    {
        this.authService = authService;
        this.logger = logger;
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
        if (!this.ModelState.IsValid)
        {
            return this.View(model);
        }

        try
        {
            var result = await this.authService.LoginAsync(model);
            if (result.Success)
            {
                await this.SignInUser(result.Token);
                return this.RedirectToAction("Index", "TodoList");
            }

            this.ModelState.AddModelError(string.Empty, result.Message);
        }
        catch (Exception ex)
        {
            AuthLoggingMessages.ErrorSigninIn(this.logger, ex.Message, ex);
            throw;
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
        if (!this.ModelState.IsValid)
        {
            return this.View(model);
        }

        try
        {
            var result = await this.authService.RegisterAsync(model);
            if (result.Success)
            {
                await this.SignInUser(result.Token);
                return this.RedirectToAction("Index", "TodoList");
            }

            this.ModelState.AddModelError(string.Empty, result.Message);
        }
        catch (Exception ex)
        {
            AuthLoggingMessages.ErrorWhileRegistering(this.logger, ex.Message, ex);
            throw;
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
        try
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
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(6000),
            };

            await this.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }
        catch (Exception ex)
        {
            AuthLoggingMessages.ErrorWhileSigningInUser(this.logger, ex.Message, ex);
            throw;
        }
    }
}

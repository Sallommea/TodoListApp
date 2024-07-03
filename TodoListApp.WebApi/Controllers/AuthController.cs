using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.Services.Database;
using TodoListApp.WebApi.Models.Auth;
using TodoListApp.WebApi.Utilities;

namespace TodoListApp.WebApi.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> userManager;
    private readonly SignInManager<User> signInManager;
    private readonly JwtGenerator jwtGenerator;

    public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, JwtGenerator jwtGenerator)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.jwtGenerator = jwtGenerator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterModel model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState);
        }

        var user = new User
        {
            UserName = model.Email,
            Email = model.Email,
            Firstname = model.Firstname,
            Lastname = model.Lastname,
        };

        var result = await this.userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            var token = this.jwtGenerator.GenerateToken(user);

            return this.Ok(new { Token = token, Message = "User registered successfully" });
        }

        foreach (var error in result.Errors)
        {
            this.ModelState.AddModelError(string.Empty, error.Description);
        }

        return this.BadRequest(this.ModelState);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginModel model)
    {
        var user = await this.userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return this.Unauthorized("Invalid email or password.");
        }

        var result = await this.signInManager.CheckPasswordSignInAsync(user, model.Password, false);
        if (result.Succeeded)
        {
            var token = this.jwtGenerator.GenerateToken(user);
            return this.Ok(new { Token = token, Message = "Login successful" });
        }

        return this.Unauthorized("Invalid email or password.");
    }
}

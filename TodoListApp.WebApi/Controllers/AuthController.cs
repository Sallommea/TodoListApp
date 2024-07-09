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
    private readonly ILogger<AuthController> logger;

    public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, JwtGenerator jwtGenerator, ILogger<AuthController> logger)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.jwtGenerator = jwtGenerator;
        this.logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterModel model)
    {
        try
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

                return this.Ok(new AuthResult { Success = true, Token = token, Message = "User registered successfully" });
            }

            foreach (var error in result.Errors)
            {
                if (error.Code == "DuplicateEmail")
                {
                    return this.Conflict(new AuthResult { Success = false, Message = "Email already in use" });
                }

                this.ModelState.AddModelError(string.Empty, error.Description);
            }

            return this.BadRequest(new AuthResult { Success = false, Message = "Registration failed" });
        }
        catch (InvalidOperationException ioe)
        {
            Logging.AuthLoggerMessages.InvalidOperationWhileRegistering(this.logger, ioe.Message, ioe);
            return this.StatusCode(StatusCodes.Status500InternalServerError, "An invalid operation occured while registering an user");
        }
        catch (Exception ex)
        {
            Logging.AuthLoggerMessages.UnexpectedErrorOccurredWhileRegistering(this.logger, ex.Message, ex);
            throw;
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginModel model)
    {
        try
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
        catch (InvalidOperationException ioe)
        {
            Logging.AuthLoggerMessages.InvalidOperationWhileSigningIn(this.logger, ioe.Message, ioe);
            return this.StatusCode(StatusCodes.Status500InternalServerError, "An invalid operation occured while signing in");
        }
        catch (Exception ex)
        {
            Logging.AuthLoggerMessages.UnexpectedErrorOccurredWhileSigningIn(this.logger, ex.Message, ex);
            throw;
        }
    }
}

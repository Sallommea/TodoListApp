using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApp.Models;
#pragma warning disable S6967 // ModelState.IsValid should be called in controller actions

namespace TodoListApp.WebApp.Controllers;
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return this.View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(string? message = null)
    {
        var viewModel = new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier,
            ErrorMessage = message ?? "An error occurred while processing your request.",
        };
        return this.View(viewModel);
    }
}

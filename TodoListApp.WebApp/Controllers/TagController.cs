using Microsoft.AspNetCore.Mvc;
using TodoListApp.Services.WebApi.Services;

namespace TodoListApp.WebApp.Controllers;
public class TagController : Controller
{
    private readonly TagWebApiService tagWebApiService;
    private readonly ILogger<TagController> logger;

    public TagController(TagWebApiService tagWebApiService, ILogger<TagController> logger)
    {
        this.tagWebApiService = tagWebApiService;
        this.logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> AddTagToTask(int taskId, string tagName)
    {
        if (string.IsNullOrWhiteSpace(tagName))
        {
            this.TempData["ErrorMessage"] = "Tag name cannot be empty.";
            return this.RedirectToAction("TaskDetails", "Task", new { taskId });
        }

        try
        {
            _ = await this.tagWebApiService.AddTagToTaskAsync(tagName, taskId);

            this.TempData["SuccessMessage"] = "Tag added successfully.";
        }
        catch (Exception ex)
        {
            this.TempData["ErrorMessage"] = $"An unexpected error occurred: {ex.Message}";
        }

        return this.RedirectToAction("TaskDetails", "Task", new { taskId });
    }
}

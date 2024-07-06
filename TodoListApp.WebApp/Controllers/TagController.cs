using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.Services.WebApi.Services;
using TodoListApp.WebApp.Logging;
using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Controllers;
public class TagController : Controller
{
    private readonly TagWebApiService tagWebApiService;
    private readonly TaskWebApiService taskWebApiService;
    private readonly ILogger<TagController> logger;

    public TagController(TagWebApiService tagWebApiService, TaskWebApiService taskWebApiService, ILogger<TagController> logger)
    {
        this.tagWebApiService = tagWebApiService;
        this.taskWebApiService = taskWebApiService;
        this.logger = logger;
    }

#pragma warning disable S6967 // ModelState.IsValid should be called in controller actions

    public async Task<IActionResult> Index(int? tagId = null, int pageNumber = 1, int pageSize = 5)
    {
        try
        {
            var token = this.User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(token))
            {
                return this.RedirectToAction("Login", "Account");
            }

            var tags = await this.tagWebApiService.GetAllTagsAsync(token);
            var model = new TagIndexViewModel
            {
                Tags = tags,
                SelectedTagId = tagId,
                CurrentPage = pageNumber,
                PageSize = pageSize,
            };

            if (tagId.HasValue)
            {
                var paginatedTasks = await this.taskWebApiService.GetTasksByTagAsync(tagId.Value, token, pageNumber, pageSize);
                model.Tasks = paginatedTasks.ResultList!;
                model.TotalPages = paginatedTasks.TotalPages ?? 0;
                model.TotalRecord = paginatedTasks.TotalRecords ?? 0;
            }

            return this.View(model);
        }
        catch (HttpRequestException ex)
        {
            TagLoggerMessages.HTTPErrorWhileGettingTags(this.logger, ex.Message, ex);
            this.ViewBag.ErrorMessage = "An error occurred while fetching the tags. Please try again later.";
        }
        catch (InvalidOperationException ioe)
        {
            TagLoggerMessages.IOEGettingTags(this.logger, ioe.Message, ioe);
            this.ViewBag.ErrorMessage = "An invalid operation occurred while fetching the tags.";
        }
        catch (Exception ex)
        {
            TagLoggerMessages.ErrorGettingTags(this.logger, ex.Message, ex);
            this.ViewBag.ErrorMessage = "An error occurred while fetching data. Please try again later.";
            throw;
        }

        return this.View(new TagIndexViewModel());
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
            var token = this.User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(token))
            {
                return this.RedirectToAction("Login", "Account");
            }

            _ = await this.tagWebApiService.AddTagToTaskAsync(tagName, taskId, token);

            this.TempData["SuccessMessage"] = "Tag added successfully.";
        }
        catch (HttpRequestException ex)
        {
            TagLoggerMessages.HTTPErrorAddingTagToTask(this.logger, ex.Message, ex);
            this.TempData["ErrorMessage"] = "An error occured while adding tag to task";
        }
        catch (InvalidOperationException ioe)
        {
            TagLoggerMessages.IOEAddingTagToTask(this.logger, ioe.Message, ioe);
            return this.StatusCode(StatusCodes.Status500InternalServerError, new { message = "An invalid operation occurred: while adding tag to task" });
        }
        catch (Exception ex)
        {
            this.TempData["ErrorMessage"] = $"An unexpected error occurred: {ex.Message}";
            TagLoggerMessages.ErrorAddingTagToTask(this.logger, ex.Message, ex);
            throw;
        }

        return this.RedirectToAction("TaskDetails", "Task", new { taskId });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteTag(int taskId, int tagId)
    {
        try
        {
            var token = this.User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(token))
            {
                return this.RedirectToAction("Login", "Account");
            }

            await this.tagWebApiService.DeleteTagAsync(taskId, tagId, token);
        }
        catch (HttpRequestException ex)
        {
            TagLoggerMessages.HttpExceptionOccurredWhileDeletingTag(this.logger, ex.Message, tagId, ex);

            return this.StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while deleting tag.");
        }
        catch (InvalidOperationException ioe)
        {
            TagLoggerMessages.IOEDeletingTag(this.logger, ioe.Message, tagId, ioe);
            return this.StatusCode(StatusCodes.Status500InternalServerError, "An invalid operation occured while deleting tag");
        }
        catch (Exception ex)
        {
            TagLoggerMessages.ErrorDeletingTag(this.logger, ex.Message, tagId, ex);
            throw;
        }

        return this.RedirectToAction("TaskDetails", "Task", new { taskId });
    }
}

using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.Services.WebApi.Services;
using TodoListApp.WebApi.Models.Tasks;
using TodoListApp.WebApp.Logging;
using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Controllers;
public class AssignedTasksController : Controller
{
    private readonly AssignedTaskWebApiService assignedTaskWebApiService;
    private readonly ILogger<AssignedTasksController> logger;

    public AssignedTasksController(AssignedTaskWebApiService assignedTaskWebApiService, ILogger<AssignedTasksController> logger)
    {
        this.assignedTaskWebApiService = assignedTaskWebApiService;
        this.logger = logger;
    }

#pragma warning disable S6967 // ModelState.IsValid should be called in controller actions
    public async Task<IActionResult> Index(int pageNumber = 1, int tasksPerPage = 6, Status? status = null, string? sortCriteria = null)
    {
        try
        {
            var token = this.User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(token))
            {
                return this.RedirectToAction("Login", "Account");
            }

            bool resetPagination = false;

            var currentStatus = this.TempData["CurrentStatus"] as int?;

            if (status != (currentStatus.HasValue ? (Status?)currentStatus.Value : null))
            {
                resetPagination = true;
            }

            var currentSortCriteria = this.TempData["CurrentSortCriteria"] as string;
            if (!string.Equals(sortCriteria, currentSortCriteria, StringComparison.OrdinalIgnoreCase))
            {
                resetPagination = true;
            }

            if (resetPagination)
            {
                pageNumber = 1;
            }

            this.TempData["CurrentStatus"] = status.HasValue ? (int)status.Value : (int?)null;
            this.TempData["CurrentSortCriteria"] = sortCriteria;

            this.ViewBag.CurrentPage = pageNumber;
            this.ViewBag.TasksPerPage = tasksPerPage;
            this.ViewBag.Status = status;
            this.ViewBag.SortCriteria = sortCriteria;

            var result = await this.assignedTaskWebApiService.GetTasksAssignedToMeAsync(token, pageNumber, tasksPerPage, status, sortCriteria);
            return this.View(result);
        }
        catch (HttpRequestException)
        {
            return this.View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }
        catch (InvalidOperationException ioe)
        {
            AssignedTasksLoggerMessages.IOErrorWhileFetchingAssignedTasks(this.logger, ioe.Message, ioe);
            return this.StatusCode(StatusCodes.Status500InternalServerError, new { message = "An invalid operation occurred " });
        }
        catch (Exception ex)
        {
            AssignedTasksLoggerMessages.ErrorWhileFetchingAssignedTasks(this.logger, ex.Message, ex);
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> ChangeStatus(int taskId, Status newStatus, int pageNumber = 1, int tasksPerPage = 2, Status? status = null, string? sortCriteria = null)
    {
        try
        {
            var token = this.User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(token))
            {
                return this.RedirectToAction("Login", "Account");
            }

            var updateTaskStatus = new UpdateTaskStatus
            {
                TaskId = taskId,
                Status = newStatus,
            };

            await this.assignedTaskWebApiService.UpdateTaskStatusAsync(updateTaskStatus, token);

            return this.RedirectToAction(nameof(this.Index), new { pageNumber, tasksPerPage, status, sortCriteria });
        }
        catch (HttpRequestException)
        {
            return this.View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }
        catch (InvalidOperationException ioe)
        {
            AssignedTasksLoggerMessages.IOEWhileChangingStatus(this.logger, taskId, ioe.Message, ioe);
            return this.StatusCode(StatusCodes.Status500InternalServerError, new { message = "An invalid operation occurred while changing task status" });
        }
        catch (Exception ex)
        {
            AssignedTasksLoggerMessages.ErrorWhileChangingStatus(this.logger, taskId, ex.Message, ex);
            throw;
        }
    }
}

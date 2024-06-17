using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.Services.WebApi.Services;
using TodoListApp.WebApi.Models.Tasks;
using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Controllers;
public class AssignedTasksController : Controller
{
    private readonly AssignedTaskWebApiService assignedTaskWebApiService;

    public AssignedTasksController(AssignedTaskWebApiService assignedTaskWebApiService)
    {
        this.assignedTaskWebApiService = assignedTaskWebApiService;
    }

    public async Task<IActionResult> Index(int pageNumber = 1, int tasksPerPage = 2, Status? status = null, string? sortCriteria = null)
    {
        try
        {
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

            var result = await this.assignedTaskWebApiService.GetTasksAssignedToMeAsync(pageNumber, tasksPerPage, status, sortCriteria);
            return this.View(result);
        }
        catch (HttpRequestException)
        {
            return this.View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }
    }

    [HttpPost]
    public async Task<IActionResult> ChangeStatus(int taskId, Status newStatus, int pageNumber = 1, int tasksPerPage = 2, Status? status = null, string? sortCriteria = null)
    {
        try
        {
            var updateTaskStatus = new UpdateTaskStatus
            {
                TaskId = taskId,
                Status = newStatus,
            };

            await this.assignedTaskWebApiService.UpdateTaskStatusAsync(updateTaskStatus);

            return this.RedirectToAction(nameof(this.Index), new { pageNumber, tasksPerPage, status, sortCriteria });
        }
        catch (Exception)
        {
            return this.View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }
    }
}

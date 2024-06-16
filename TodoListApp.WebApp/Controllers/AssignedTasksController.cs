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

    public async Task<IActionResult> Index(int pageNumber = 1, int tasksPerPage = 10, Status? status = null, string? sortCriteria = null)
    {
        try
        {
            if (!status.HasValue && this.TempData["CurrentStatus"] != null)
            {
                status = (Status?)(this.TempData["CurrentStatus"] as int?);
            }

            if (string.IsNullOrEmpty(sortCriteria) && this.TempData["CurrentSortCriteria"] != null)
            {
                sortCriteria = this.TempData["CurrentSortCriteria"] as string;
            }

            // Store the current status and sort criteria for the next request
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
}

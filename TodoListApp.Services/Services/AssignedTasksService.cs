using Microsoft.Extensions.Logging;
using TodoListApp.Services.Database.Models;
using TodoListApp.Services.Database.Repositories;
using TodoListApp.Services.Interfaces;
using TodoListApp.WebApi.Models.Tasks;

namespace TodoListApp.Services.Services;
public class AssignedTasksService : IAssignedTasksService
{
    private readonly IAssignedTasksRepository assignedTasksRepository;
    private readonly ILogger<AssignedTasksService> logger;

    public AssignedTasksService(IAssignedTasksRepository assignedTasksRepository, ILogger<AssignedTasksService> logger)
    {
        this.assignedTasksRepository = assignedTasksRepository;
        this.logger = logger;
    }

    public async Task<PaginatedListResult<TaskDetailsDto>> GetTasksByAssigneeAsync(string assignee, int pageNumber, int tasksPerPage, Status? status = null, string? sortCriteria = null)
    {
        try
        {
            var tasks = await this.assignedTasksRepository.GetTasksByAssigneeAsync(assignee, pageNumber, tasksPerPage, (Database.Status?)status, sortCriteria);

            if (tasks.TotalRecords == 0)
            {
                return new PaginatedListResult<TaskDetailsDto>
                {
                    TotalRecords = 0,
                    TotalPages = 0,
                    ResultList = new List<TaskDetailsDto>(),
                };
            }

            var currentDate = DateTime.UtcNow;

            foreach (var t in tasks.ResultList!)
            {
                t.IsExpired = t.DueDate.HasValue && t.DueDate.Value < currentDate;
            }

            await this.assignedTasksRepository.SaveChangesAsync();

            var taskDetailsDtos = tasks.ResultList.Select(t => new TaskDetailsDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                CreatedDate = t.CreatedDate,
                DueDate = t.DueDate,
                Status = (Status)t.Status,
                Assignee = t.Assignee,
                TodoListId = t.TodoListId,
                IsExpired = t.IsExpired,
            }).ToList();

            return new PaginatedListResult<TaskDetailsDto>
            {
                TotalRecords = tasks.TotalRecords,
                TotalPages = tasks.TotalPages,
                ResultList = taskDetailsDtos,
            };
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "An error occurred while retrieving tasks for assignee {Assignee}.", assignee);
            throw;
        }
    }

    public async Task<bool> UpdateTaskStatusAsync(UpdateTaskStatus updateTaskStatusDto)
    {
        try
        {
            var result = await this.assignedTasksRepository.UpdateTaskStatusAsync(updateTaskStatusDto.TaskId, (Database.Status)updateTaskStatusDto.Status);
            if (!result)
            {
                this.logger.LogWarning("Task with ID {TaskId} not found for status update.", updateTaskStatusDto.TaskId);
                return false;
            }

            this.logger.LogInformation("Task with ID {TaskId} status updated to {Status}.", updateTaskStatusDto.TaskId, updateTaskStatusDto.Status);
            return true;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "An error occurred while updating the status of task with ID {TaskId}.", updateTaskStatusDto.TaskId);
            throw;
        }
    }
}

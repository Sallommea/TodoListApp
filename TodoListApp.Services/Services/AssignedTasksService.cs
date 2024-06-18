using Microsoft.Extensions.Logging;
using TodoListApp.Services.Database.Models;
using TodoListApp.Services.Database.Repositories;
using TodoListApp.Services.Interfaces;
using TodoListApp.Services.Logging;
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

    public async Task<PaginatedListResult<AssignedTasksdto>> GetTasksByAssigneeAsync(string assignee, int pageNumber, int tasksPerPage, Status? status = null, string? sortCriteria = null)
    {
        try
        {
            var tasks = await this.assignedTasksRepository.GetTasksByAssigneeAsync(assignee, pageNumber, tasksPerPage, (Database.Status?)status, sortCriteria);

            if (tasks.TotalRecords == 0)
            {
                return new PaginatedListResult<AssignedTasksdto>
                {
                    TotalRecords = 0,
                    TotalPages = 0,
                    ResultList = new List<AssignedTasksdto>(),
                };
            }

            var currentDate = DateTime.UtcNow;

            foreach (var t in tasks.ResultList!)
            {
                t.IsExpired = t.DueDate.HasValue && t.DueDate.Value < currentDate;
            }

            await this.assignedTasksRepository.SaveChangesAsync();

            var taskDetailsDtos = tasks.ResultList.Select(t => new AssignedTasksdto
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

            return new PaginatedListResult<AssignedTasksdto>
            {
                TotalRecords = tasks.TotalRecords,
                TotalPages = tasks.TotalPages,
                ResultList = taskDetailsDtos,
            };
        }
        catch (Exception ex)
        {
            AssignedTasksLoggerMessages.UnexpectedErrorOccurredWhileGettingUserTasks(this.logger, assignee, ex);
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
                AssignedTasksLoggerMessages.InvalidTaskIdForTaskStatusUpdate(this.logger, updateTaskStatusDto.TaskId);
                return false;
            }

            AssignedTasksLoggerMessages.TaskStatusUpdatedSuccessfully(this.logger, updateTaskStatusDto.TaskId);
            return true;
        }
        catch (Exception ex)
        {
            AssignedTasksLoggerMessages.UnexpectedErrorOccurredWhileUpdatingTaskStatus(this.logger, ex.Message, ex);
            throw;
        }
    }
}

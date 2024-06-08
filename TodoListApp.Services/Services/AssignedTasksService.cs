using TodoListApp.Services.Database.Models;
using TodoListApp.Services.Database.Repositories;
using TodoListApp.Services.Interfaces;
using TodoListApp.WebApi.Models.Tasks;

namespace TodoListApp.Services.Services;
public class AssignedTasksService : IAssignedTasksService
{
    private readonly IAssignedTasksRepository assignedTasksRepository;

    public AssignedTasksService(IAssignedTasksRepository assignedTasksRepository)
    {
        this.assignedTasksRepository = assignedTasksRepository;
    }

    public async Task<PaginatedListResult<TaskDetailsDto>> GetTasksByAssigneeAsync(string assignee, int pageNumber, int tasksPerPage, Status? status = null, string? sortCriteria = null)
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

    public async Task<bool> UpdateTaskStatusAsync(UpdateTaskStatus updateTaskStatusDto)
    {
        return await this.assignedTasksRepository.UpdateTaskStatusAsync(updateTaskStatusDto.TaskId, (Database.Status)updateTaskStatusDto.Status);
    }
}

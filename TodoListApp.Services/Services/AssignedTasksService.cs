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

    public async Task<List<TaskDetailsDto>> GetTasksByAssigneeAsync(string assignee, Status? status = null, string? sortCriteria = null)
    {
        var tasks = await this.assignedTasksRepository.GetTasksByAssigneeAsync(assignee, (Database.Status?)status, sortCriteria);

        var currentDate = DateTime.UtcNow;

        foreach (var t in tasks)
        {
            if (t.DueDate.HasValue && t.DueDate.Value < currentDate)
            {
                t.IsExpired = true;
            }
            else
            {
                t.IsExpired = false;
            }
        }

        await this.assignedTasksRepository.SaveChangesAsync();

        return tasks.Select(t => new TaskDetailsDto
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
    }

    public async Task<bool> UpdateTaskStatusAsync(UpdateTaskStatus updateTaskStatusDto)
    {
        return await this.assignedTasksRepository.UpdateTaskStatusAsync(updateTaskStatusDto.TaskId, (Database.Status)updateTaskStatusDto.Status);
    }
}

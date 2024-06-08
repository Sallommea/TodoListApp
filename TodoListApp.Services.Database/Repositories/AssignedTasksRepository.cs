using Microsoft.EntityFrameworkCore;

namespace TodoListApp.Services.Database.Repositories;
public class AssignedTasksRepository : IAssignedTasksRepository
{
    private readonly TodoListDbContext dbContext;

    public AssignedTasksRepository(TodoListDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<List<TaskEntity>> GetTasksByAssigneeAsync(string assignee, Status? status = null, string? sortCriteria = null)
    {
        var query = this.dbContext.Tasks.AsQueryable();

        if (!status.HasValue)
        {
            query = query.Where(t => t.Status == Status.NotStarted || t.Status == Status.InProgress);
        }
        else
        {
            query = query.Where(t => t.Status == status.Value);
        }

        if (!string.IsNullOrEmpty(sortCriteria))
        {
            switch (sortCriteria.ToLower(System.Globalization.CultureInfo.CurrentCulture))
            {
                case "duedate":
                    query = query
                    .OrderBy(t => t.DueDate == null)
                    .ThenBy(t => t.DueDate);
                    break;
                case "title":
                    query = query.OrderBy(t => t.Title);
                    break;
                default:
                    break;
            }
        }

        return await query
            .Where(t => t.Assignee == assignee)
            .ToListAsync();
    }

    public async Task<bool> UpdateTaskStatusAsync(int taskId, Status newStatus)
    {
        var task = await this.dbContext.Tasks.FindAsync(taskId);
        if (task == null)
        {
            return false;
        }

        task.Status = newStatus;
        _ = await this.dbContext.SaveChangesAsync();
        return true;
    }

    public async Task SaveChangesAsync()
    {
        _ = await this.dbContext.SaveChangesAsync();
    }
}

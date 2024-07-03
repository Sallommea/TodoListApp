using Microsoft.EntityFrameworkCore;
using TodoListApp.Services.Database.Models;

namespace TodoListApp.Services.Database.Repositories;
public class AssignedTasksRepository : IAssignedTasksRepository
{
    private readonly TodoListDbContext dbContext;

    public AssignedTasksRepository(TodoListDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<PaginatedListResult<TaskEntity>> GetTasksByAssigneeAsync(string userId, int pageNumber, int tasksPerPage, Status? status = null, string? sortCriteria = null)
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
        else
        {
            query = query.OrderByDescending(t => t.Id);
        }

        query = query.Where(t => t.UserId == userId);

        var totalRecords = await query.CountAsync();

        if (totalRecords == 0)
        {
            return new PaginatedListResult<TaskEntity>
            {
                TotalRecords = 0,
                TotalPages = 0,
                ResultList = new List<TaskEntity>(),
            };
        }

        var paginatedTasks = await query
       .Skip((pageNumber - 1) * tasksPerPage)
       .Take(tasksPerPage)
       .ToListAsync();

        var totalPages = (int)Math.Ceiling((double)totalRecords / tasksPerPage);

        return new PaginatedListResult<TaskEntity>
        {
            TotalRecords = totalRecords,
            TotalPages = totalPages,
            ResultList = paginatedTasks,
        };
    }

    public async Task<bool> UpdateTaskStatusAsync(int taskId, Status newStatus, string userId)
    {
        var task = await this.dbContext.Tasks
            .FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);

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

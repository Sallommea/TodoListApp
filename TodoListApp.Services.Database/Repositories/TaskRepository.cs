using System.ComponentModel.Design;
using Microsoft.EntityFrameworkCore;
using TodoListApp.Services.Database.Models;

namespace TodoListApp.Services.Database.Repositories;
public class TaskRepository : ITaskRepository
{
    private readonly TodoListDbContext dbContext;

    public TaskRepository(TodoListDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<TaskEntity> AddTaskAsync(TaskEntity task, string userId)
    {
        task.UserId = userId;
        _ = await this.dbContext.Tasks.AddAsync(task);

        var todoList = await this.dbContext.TodoLists
              .FirstOrDefaultAsync(tl => tl.Id == task.TodoListId && tl.UserId == userId);
        if (todoList != null)
        {
            todoList.TaskCount++;
            _ = this.dbContext.TodoLists.Update(todoList);
        }
        else
        {
            throw new KeyNotFoundException($"TodoList with ID {task.TodoListId} not found while adding task.");
        }

        _ = await this.dbContext.SaveChangesAsync();
        return task;
    }

    public async Task<bool> DeleteTaskAsync(int taskId, string userId)
    {
        var task = await this.dbContext.Tasks
           .FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);
        if (task == null)
        {
            return false;
        }

        _ = this.dbContext.Tasks.Remove(task);

        var todoList = await this.dbContext.TodoLists
           .FirstOrDefaultAsync(tl => tl.Id == task.TodoListId && tl.UserId == userId);
        if (todoList != null && todoList.TaskCount > 0)
        {
            todoList.TaskCount--;
            _ = this.dbContext.TodoLists.Update(todoList);
        }

        _ = await this.dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<TaskEntity?> GetTaskByIdAsync(int taskId, string userId)
    {
        return await this.dbContext.Tasks
         .Include(t => t.TaskTags)
        .ThenInclude(tt => tt.Tag)
         .Include(t => t.Comments)
         .ThenInclude(c => c.User)
        .FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);
    }

    public async Task<PaginatedListResult<TaskEntity>> GetTasksByTagIdAsync(int tagId, string userId, int pageNumber, int pageSize)
    {
        var query = this.dbContext.Tasks
            .Include(t => t.TaskTags)
            .ThenInclude(tt => tt.Tag)
            .Where(t => t.TaskTags.Any(tt => tt.TagId == tagId && t.UserId == userId))
            .OrderByDescending(t => t.CreatedDate);

        var totalCount = await query.CountAsync();

        var tasks = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedListResult<TaskEntity>
        {
            ResultList = tasks,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
            TotalRecords = totalCount,
        };
    }

    public async Task UpdateTaskAsync(int taskId, TaskEntity task, string userId)
    {
        var existingTask = await this.dbContext.Tasks
            .FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);

        if (existingTask == null)
        {
            throw new KeyNotFoundException($"Task with ID {taskId} not found in TodoList.");
        }

        existingTask.Title = task.Title;
        existingTask.Description = task.Description;
        existingTask.DueDate = task.DueDate;
        existingTask.Status = task.Status;
        existingTask.IsExpired = task.IsExpired;
        _ = await this.dbContext.SaveChangesAsync();
    }

    public async Task<PaginatedListResult<TaskEntity>> SearchTasksByTitleAsync(int pageNumber, int tasksPerPage, string searchText, string userId)
    {
        var result = new PaginatedListResult<TaskEntity>();
        var query = this.dbContext.Tasks.Where(t => t.UserId == userId);

        if (!string.IsNullOrEmpty(searchText))
        {
            query = query.Where(t => t.Title.Contains(searchText));
        }

        result.TotalRecords = await query.CountAsync();
        result.TotalPages = (int)Math.Ceiling((double)result.TotalRecords / tasksPerPage);
        result.ResultList = await query
            .OrderByDescending(x => x.Id)
            .Skip((pageNumber - 1) * tasksPerPage)
            .Take(tasksPerPage)
            .ToListAsync();

        return result;
    }

    public async Task AddCommentAsync(CommentEntity comment, string userId)
    {
        comment.UserId = userId;
        _ = await this.dbContext.Comments.AddAsync(comment);
        _ = await this.dbContext.SaveChangesAsync();
    }

    public async Task<CommentEntity?> GetCommentByIdAsync(int commentId, string userId)
    {
        return await this.dbContext.Comments
         .FirstOrDefaultAsync(c => c.Id == commentId && c.UserId == userId);
    }

    public async Task UpdateCommentAsync(CommentEntity comment, string userId)
    {
        var existingComment = await this.dbContext.Comments
          .FirstOrDefaultAsync(c => c.Id == comment.Id && c.UserId == userId);
        if (existingComment == null)
        {
            throw new KeyNotFoundException($"Comment with ID {comment.Id} not found for this user.");
        }

        existingComment.Content = comment.Content;
        _ = await this.dbContext.SaveChangesAsync();
    }

    public async Task DeleteCommentAsync(int commentId, string userId)
    {
        var comment = await this.dbContext.Comments
          .FirstOrDefaultAsync(c => c.Id == commentId && c.UserId == userId);

        if (comment == null)
        {
            throw new KeyNotFoundException($"Comment with ID {commentId} not found for this user.");
        }

        _ = this.dbContext.Comments.Remove(comment);
        _ = await this.dbContext.SaveChangesAsync();
    }
}

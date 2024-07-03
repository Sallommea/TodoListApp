using TodoListApp.Services.Database.Models;

namespace TodoListApp.Services.Database.Repositories;
public interface ITaskRepository
{
    Task<TaskEntity?> GetTaskByIdAsync(int taskId, string userId);

    Task<TaskEntity> AddTaskAsync(TaskEntity task, string userId);

    Task<PaginatedListResult<TaskEntity>> GetTasksByTagIdAsync(int tagId, string userId, int pageNumber, int pageSize);

    Task UpdateTaskAsync(int taskId, TaskEntity task, string userId);

    Task<bool> DeleteTaskAsync(int taskId, string userId);

    Task<PaginatedListResult<TaskEntity>> SearchTasksByTitleAsync(int pageNumber, int tasksPerPage, string searchText, string userId);

    Task AddCommentAsync(CommentEntity comment, string userId);

    Task<CommentEntity?> GetCommentByIdAsync(int commentId, string userId);

    Task UpdateCommentAsync(CommentEntity comment, string userId);

    Task DeleteCommentAsync(int commentId, string userId);
}

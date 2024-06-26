using TodoListApp.Services.Database.Models;

namespace TodoListApp.Services.Database.Repositories;
public interface ITaskRepository
{
    Task<TaskEntity?> GetTaskByIdAsync(int taskId);

    Task<TaskEntity> AddTaskAsync(TaskEntity task);

    Task<PaginatedListResult<TaskEntity>> GetTasksByTagIdAsync(int tagId, int pageNumber, int pageSize);

    Task UpdateTaskAsync(int taskId, TaskEntity task);

    Task<bool> DeleteTaskAsync(int taskId);

    Task<PaginatedListResult<TaskEntity>> SearchTasksByTitleAsync(int pageNumber, int tasksPerPage, string searchText);

    Task AddCommentAsync(CommentEntity comment);

    Task<CommentEntity?> GetCommentByIdAsync(int commentId);

    Task UpdateCommentAsync(CommentEntity comment);

    Task DeleteCommentAsync(CommentEntity comment);
}

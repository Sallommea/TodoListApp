using TodoListApp.Services.Models;
using TodoListApp.WebApi.Models.Comments;
using TodoListApp.WebApi.Models.Tasks;

namespace TodoListApp.Services.Interfaces;
public interface ITaskService
{
    Task<TaskDetailsDto?> GetTaskDetailsAsync(int taskId, string userId);

    Task<PaginatedListResult<TaskDto>> GetTasksByTagIdAsync(int tagId, string userId, int pageNumber, int pageSize);

    Task<TaskDetailsDto> CreateTaskAsync(CreateTaskDto createTaskDto, string userId);

    Task<bool> DeleteTaskAsync(int taskId, string userId);

    Task<bool> UpdateTaskAsync(int taskId, UpdateTaskDto updateTaskDto, string userId);

    Task<PaginatedListResult<TaskSearchResultDto>> GetPaginatedSearchedTasksAsync(int pageNumber, int itemsPerPage, string searchText, string userId);

    Task<CommentDto> AddCommentAsync(AddCommentDto addCommentDto, string userId);

    Task<CommentDto> EditCommentAsync(EditCommentDto editCommentDto, string userId);

    Task DeleteCommentAsync(int taskId, int commentId, string userId);
}

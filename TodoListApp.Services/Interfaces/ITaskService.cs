using TodoListApp.Services.Models;
using TodoListApp.WebApi.Models.Comments;
using TodoListApp.WebApi.Models.Tasks;

namespace TodoListApp.Services.Interfaces;
public interface ITaskService
{
    Task<TaskDetailsDto?> GetTaskDetailsAsync(int taskId);

    Task<PaginatedListResult<TaskDto>> GetTasksByTagIdAsync(int tagId, int pageNumber, int pageSize);

    Task<TaskDetailsDto> CreateTaskAsync(CreateTaskDto createTaskDto);

    Task<bool> DeleteTaskAsync(int taskId);

    Task<bool> UpdateTaskAsync(int taskId, UpdateTaskDto updateTaskDto);

    Task<PaginatedListResult<TaskSearchResultDto>> GetPaginatedSearchedTasksAsync(int pageNumber, int itemsPerPage, string searchText);

    Task<CommentDto> AddCommentAsync(AddCommentDto addCommentDto);
}

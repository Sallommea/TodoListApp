using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using TodoListApp.Services.Models;
using TodoListApp.Services.WebApi.Logging;
using TodoListApp.WebApi.Models.Comments;
using TodoListApp.WebApi.Models.Tasks;

namespace TodoListApp.Services.WebApi.Services;
public class TaskWebApiService
{
    private readonly HttpClient httpClient;
    private readonly ILogger<TaskWebApiService> logger;

    public TaskWebApiService(HttpClient httpClient, ILogger<TaskWebApiService> logger)
    {
        this.httpClient = httpClient;
        this.logger = logger;
    }

    public async Task<int> AddTaskAsync(CreateTaskDto createTask)
    {
        try
        {
            var response = await this.httpClient.PostAsJsonAsync("api/Tasks", createTask);
            _ = response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<int>();
        }
        catch (HttpRequestException ex)
        {
            TaskServiceLoggerMessages.HTTPErrorWhileAddingTask(this.logger, ex.Message, ex);
            throw;
        }
        catch (Exception ex)
        {
            TaskServiceLoggerMessages.ErrorAddingTask(this.logger, ex.Message, ex);
            throw;
        }
    }

    public async Task DeleteTaskAsync(int taskId)
    {
        try
        {
            var response = await this.httpClient.DeleteAsync($"api/Tasks/{taskId}");
            _ = response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            TaskServiceLoggerMessages.HTTPErrorWhileDeletingTask(this.logger, taskId, ex.Message, ex);
            throw;
        }
        catch (Exception ex)
        {
            TaskServiceLoggerMessages.ErrorDeletingTask(this.logger, taskId, ex.Message, ex);
            throw;
        }
    }

    public async Task<TaskDetailsDto> GetTaskDetailsAsync(int taskId)
    {
        try
        {
            var response = await this.httpClient.GetAsync($"api/Tasks/{taskId}");

            var taskDetails = await response.Content.ReadFromJsonAsync<TaskDetailsDto>();

            if (taskDetails == null)
            {
                throw new InvalidOperationException("Failed to deserialize task details.");
            }

            return taskDetails;
        }
        catch (HttpRequestException ex)
        {
            TaskServiceLoggerMessages.HTTPErrorGettingTask(this.logger, taskId, ex.Message, ex);
            throw;
        }
        catch (Exception ex)
        {
            TaskServiceLoggerMessages.ErrorGettingTask(this.logger, taskId, ex.Message, ex);
            throw;
        }
    }

    public async Task UpdateTaskAsync(int taskId, UpdateTaskDto updateTaskDto)
    {
        try
        {
            var response = await this.httpClient.PutAsJsonAsync($"api/tasks/{taskId}", updateTaskDto);
            _ = response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            TaskServiceLoggerMessages.HTTPErrorUpdatingTask(this.logger, taskId, ex.Message, ex);
            throw;
        }
        catch (Exception ex)
        {
            TaskServiceLoggerMessages.ErrorUpdatingTask(this.logger, taskId, ex.Message, ex);
            throw;
        }
    }

    public async Task<PaginatedListResult<TaskSearchResultDto>> GetPaginatedSearchedTasksAsync(string searchText, int pageNumber = 1, int itemsPerPage = 10)
    {
        try
        {
            var response = await this.httpClient.GetAsync($"api/tasks/bysearchtext?searchText={Uri.EscapeDataString(searchText)}&pageNumber={pageNumber}&itemsPerPage={itemsPerPage}");

            _ = response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<PaginatedListResult<TaskSearchResultDto>>();

            return result ?? new PaginatedListResult<TaskSearchResultDto>();
        }
        catch (HttpRequestException ex)
        {
            TaskServiceLoggerMessages.HTTPErrorGettingSearchedTasks(this.logger, searchText, ex.Message, ex);
            throw;
        }
        catch (JsonException ex)
        {
            TaskServiceLoggerMessages.ParsingErrorGettingSearchedTasks(this.logger, searchText, ex.Message, ex);
            throw;
        }
        catch (Exception ex)
        {
            TaskServiceLoggerMessages.ErrorGettingSearchedTasks(this.logger, searchText, ex.Message, ex);
            throw;
        }
    }

    public async Task<PaginatedListResult<TaskDto>> GetTasksByTagAsync(int tagId, int pageNumber, int itemsPerPage)
    {
        try
        {
            var response = await this.httpClient.GetAsync($"api/Tasks/bytag/{tagId}?pageNumber={pageNumber}&pageSize={itemsPerPage}");

            if (response.IsSuccessStatusCode)
            {
                var paginatedTasks = await response.Content.ReadFromJsonAsync<PaginatedListResult<TaskDto>>();
                return paginatedTasks ?? new PaginatedListResult<TaskDto>();
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Error getting tasks by tag. Status code: {response.StatusCode}, Content: {content}");
            }
        }
        catch (HttpRequestException ex)
        {
            TaskServiceLoggerMessages.HTTPErrorWhileGettingTasksByTag(this.logger, ex.Message, ex);
            throw;
        }
        catch (JsonException ex)
        {
            TaskServiceLoggerMessages.JSONParsingErrorWhileGettingTasksByTag(this.logger, ex.Message, ex);
            throw;
        }
        catch (Exception ex)
        {
            TaskServiceLoggerMessages.ErrorGettingTasksByTag(this.logger, ex.Message, ex);
            throw;
        }
    }

    public async Task<CommentDto> AddCommentAsync(AddCommentDto addCommentDto)
    {
        try
        {
            var response = await this.httpClient.PostAsJsonAsync($"api/Tasks/{addCommentDto.TaskId}/comments", addCommentDto);

            _ = response.EnsureSuccessStatusCode();

            var commentDto = await response.Content.ReadFromJsonAsync<CommentDto>();
            if (commentDto == null)
            {
                throw new InvalidOperationException("Failed to deserialize the comment response.");
            }

            return commentDto;
        }
        catch (HttpRequestException ex)
        {
            TaskServiceLoggerMessages.HTTPErrorWhileAddingComment(this.logger, ex.Message, ex);
            throw;
        }
        catch (Exception ex)
        {
            TaskServiceLoggerMessages.ErrorGettingWhileAddingComment(this.logger, ex.Message, ex);
            throw;
        }
    }

    public async Task<CommentDto> EditCommentAsync(EditCommentDto editCommentDto)
    {
        try
        {
            var response = await this.httpClient.PutAsJsonAsync(
                $"api/Tasks/{editCommentDto.TaskId}/comments/{editCommentDto.CommentId}",
                editCommentDto);

            _ = response.EnsureSuccessStatusCode();

            var updatedComment = await response.Content.ReadFromJsonAsync<CommentDto>();
            if (updatedComment == null)
            {
                throw new InvalidOperationException("Failed to deserialize the updated comment response.");
            }

            return updatedComment;
        }
        catch (HttpRequestException ex)
        {
            TaskServiceLoggerMessages.HTTPErrorWhileEditingComment(this.logger, ex.Message, ex);
            throw;
        }
        catch (Exception ex)
        {
            TaskServiceLoggerMessages.ErrorGettingWhileEditingComment(this.logger, ex.Message, ex);
            throw;
        }
    }

    public async Task DeleteCommentAsync(int taskId, int commentId)
    {
        try
        {
            var response = await this.httpClient.DeleteAsync($"api/Tasks/{taskId}/comments/{commentId}");
            _ = response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            TaskServiceLoggerMessages.HTTPErrorWhileDeletingComment(this.logger, ex.Message, ex);
            throw;
        }
        catch (Exception ex)
        {
            TaskServiceLoggerMessages.ErrorWhileDeletingComment(this.logger, ex.Message, ex);
            throw;
        }
    }
}

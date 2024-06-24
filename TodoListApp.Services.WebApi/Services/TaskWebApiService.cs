using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using TodoListApp.Services.Models;
using TodoListApp.Services.WebApi.Logging;
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
}

using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
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
}

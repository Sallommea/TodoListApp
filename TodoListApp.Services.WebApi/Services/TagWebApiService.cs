using System.Net.Http.Json;
using System.Text.Json;
using System.Web;
using Microsoft.Extensions.Logging;
using TodoListApp.Services.WebApi.Logging;
using TodoListApp.WebApi.Models.Tags;

namespace TodoListApp.Services.WebApi.Services;
public class TagWebApiService
{
    private readonly HttpClient httpClient;
    private readonly ILogger<TagWebApiService> logger;

    public TagWebApiService(HttpClient httpClient, ILogger<TagWebApiService> logger)
    {
        this.httpClient = httpClient;
        this.logger = logger;
    }

    public async Task<IEnumerable<TagDto>> GetAllTagsAsync(string token)
    {
        try
        {
            if (!string.IsNullOrEmpty(token))
            {
                this.httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var response = await this.httpClient.GetAsync("api/Tag");

            if (response.IsSuccessStatusCode)
            {
                var tags = await response.Content.ReadFromJsonAsync<IEnumerable<TagDto>>();
                return tags ?? Enumerable.Empty<TagDto>();
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Error getting tags. Status code: {response.StatusCode}, Content: {content}");
            }
        }
        catch (HttpRequestException ex)
        {
            TagServiceLoggerMessages.HTTPErrorWhileGettingTags(this.logger, ex.Message, ex);
            throw;
        }
        catch (JsonException ex)
        {
            TagServiceLoggerMessages.JSONParsingErrorWhileGettingTags(this.logger, ex.Message, ex);
            throw;
        }
        catch (Exception ex)
        {
            TagServiceLoggerMessages.UnexpectedErrorWhileGettingTags(this.logger, ex.Message, ex);
            throw;
        }
    }

    public async Task<TagDto> AddTagToTaskAsync(string tagName, int taskId, string token)
    {
        try
        {
            if (!string.IsNullOrEmpty(token))
            {
                this.httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var response = await this.httpClient.PostAsync($"api/Tag/AddTagToTask?tagName={HttpUtility.UrlEncode(tagName)}&taskId={taskId}", null);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var tagDto = JsonSerializer.Deserialize<TagDto>(content);
                if (tagDto == null)
                {
                    throw new InvalidOperationException("Failed to deserialize task details.");
                }

                return tagDto;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new ArgumentException(errorMessage);
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Error adding tag to task: {errorMessage}");
            }
        }
        catch (JsonException jex)
        {
            throw new FormatException("Error deserializing the response", jex);
        }
        catch (HttpRequestException ex)
        {
            TagServiceLoggerMessages.HTTPErrorAddingTagToTask(this.logger, ex.Message, ex);
            throw;
        }
        catch (Exception ex)
        {
            TagServiceLoggerMessages.ErrorAddingTagToTask(this.logger, ex.Message, ex);
            throw;
        }
    }

    public async Task DeleteTagAsync(int taskId, int tagId, string token)
    {
        try
        {
            if (!string.IsNullOrEmpty(token))
            {
                this.httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var response = await this.httpClient.DeleteAsync($"api/Tag/{taskId}/tags/{tagId}");
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
}

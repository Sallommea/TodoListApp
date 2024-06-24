using System.Text.Json;
using System.Web;
using Microsoft.Extensions.Logging;
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

    public async Task<TagDto> AddTagToTaskAsync(string tagName, int taskId)
    {
        try
        {
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
                var statusCode = response.StatusCode;

                var errorMessage = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Status Code: {statusCode}");
                Console.WriteLine($"Error Message: {errorMessage}");
                throw new HttpRequestException($"Error adding tag to task: {errorMessage}");
            }
        }
        catch (JsonException jex)
        {
            throw new FormatException("Error deserializing the response", jex);
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}

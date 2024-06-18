using TodoListApp.WebApi.Models.Tags;

namespace TodoListApp.Services.Interfaces;
public interface ITagService
{
    Task<TagDto> AddTagToTaskAsync(string tagName, int taskId);
}

using TodoListApp.WebApi.Models.Tags;
using TodoListApp.WebApi.Models.Tasks;

namespace TodoListApp.Services.Interfaces;
public interface ITagService
{
    Task<IEnumerable<TagDto>> GetAllTagsAsync();

    Task<TagDto> AddTagToTaskAsync(string tagName, int taskId);

}

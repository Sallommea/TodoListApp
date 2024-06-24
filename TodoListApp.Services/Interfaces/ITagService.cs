using TodoListApp.WebApi.Models.Tags;

namespace TodoListApp.Services.Interfaces;
public interface ITagService
{
    Task<IEnumerable<TagDto>> GetAllTagsAsync();

    Task<TagDto> AddTagToTaskAsync(string tagName, int taskId);

    Task<bool> DeleteTagAsync(int taskId, int tagId);
}

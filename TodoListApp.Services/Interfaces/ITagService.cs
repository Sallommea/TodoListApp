using TodoListApp.WebApi.Models.Tags;

namespace TodoListApp.Services.Interfaces;
public interface ITagService
{
    Task<IEnumerable<TagDto>> GetAllTagsAsync(string userId);

    Task<TagDto> AddTagToTaskAsync(string tagName, int taskId, string userId);

    Task<bool> DeleteTagAsync(int taskId, int tagId, string userId);
}

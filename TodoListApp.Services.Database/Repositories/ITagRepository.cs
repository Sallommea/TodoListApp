namespace TodoListApp.Services.Database.Repositories;
public interface ITagRepository
{
    Task<IEnumerable<TagEntity>> GetAllTagsAsync();

    Task<TagEntity?> GetTagByNameAsync(string normalizedTagName);

    Task<TagEntity> CreateTagAsync(string normalizedTagName);

    Task AddTagToTaskAsync(int taskId, int tagId);

    Task<TaskTagEntity?> GetTaskTagAsync(int taskId, int tagId);
}

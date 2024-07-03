namespace TodoListApp.Services.Database.Repositories;
public interface ITagRepository
{
    Task<IEnumerable<TagEntity>> GetAllTagsAsync(string userId);

    Task<TagEntity?> GetTagByNameAsync(string normalizedTagName, string userId);

    Task<TagEntity> CreateTagAsync(string normalizedTagName, string userId);

    Task AddTagToTaskAsync(int taskId, int tagId);

    Task<TaskTagEntity?> GetTaskTagAsync(int taskId, int tagId);

    Task<bool> DeleteTagAsync(int taskId, int tagId, string userId);
}

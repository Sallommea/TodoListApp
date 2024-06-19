using Microsoft.EntityFrameworkCore;

namespace TodoListApp.Services.Database.Repositories;
public class TagRepository : ITagRepository
{
    private readonly TodoListDbContext context;

    public TagRepository(TodoListDbContext context)
    {
        this.context = context;
    }

    public async Task<IEnumerable<TagEntity>> GetAllTagsAsync()
    {
        return await this.context.Tags
            .Distinct()
            .ToListAsync();
    }

    public async Task<TagEntity?> GetTagByNameAsync(string normalizedTagName)
    {
        return await this.context.Tags
            .FirstOrDefaultAsync(t => t.Name == normalizedTagName);
    }

    public async Task<TagEntity> CreateTagAsync(string normalizedTagName)
    {
        var tag = new TagEntity { Name = normalizedTagName };
        _ = await this.context.Tags.AddAsync(tag);
        _ = await this.context.SaveChangesAsync();
        return tag;
    }

    public async Task AddTagToTaskAsync(int taskId, int tagId)
    {
        var taskTag = new TaskTagEntity
        {
            TaskId = taskId,
            TagId = tagId,
        };
        _ = await this.context.TaskTags.AddAsync(taskTag);
        _ = await this.context.SaveChangesAsync();
    }

    public async Task<TaskTagEntity?> GetTaskTagAsync(int taskId, int tagId)
    {
        return await this.context.TaskTags
            .FirstOrDefaultAsync(tt => tt.TaskId == taskId && tt.TagId == tagId);
    }

    public async Task<bool> DeleteTagAsync(int taskId, int tagId)
    {
        var taskTag = await this.context.TaskTags
           .FirstOrDefaultAsync(tt => tt.TaskId == taskId && tt.TagId == tagId);

        if (taskTag == null)
        {
            return false;
        }

        _ = this.context.TaskTags.Remove(taskTag);
        _ = await this.context.SaveChangesAsync();

        bool isTagUsedElsewhere = await this.context.TaskTags
            .AnyAsync(tt => tt.TagId == tagId);

        if (!isTagUsedElsewhere)
        {
            var tag = await this.context.Tags.FindAsync(tagId);
            if (tag != null)
            {
                _ = this.context.Tags.Remove(tag);
                _ = await this.context.SaveChangesAsync();
            }
        }

        return true;
    }
}

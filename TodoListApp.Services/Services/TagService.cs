using System.Globalization;
using TodoListApp.Services.Database.Repositories;
using TodoListApp.Services.Interfaces;
using TodoListApp.WebApi.Models.Tags;
using TodoListApp.WebApi.Models.Tasks;

namespace TodoListApp.Services.Services;
public class TagService : ITagService
{
    private readonly ITagRepository tagRepository;

    public TagService(ITagRepository tagRepository)
    {
        this.tagRepository = tagRepository;
    }

    public async Task<IEnumerable<TagDto>> GetAllTagsAsync()
    {
        var tags = await this.tagRepository.GetAllTagsAsync();
        return tags.Select(t => new TagDto
        {
            Id = t.Id,
            Name = t.Name,
        });
    }

    public async Task<TagDto> AddTagToTaskAsync(string tagName, int taskId)
    {
        string normalizedTagName = tagName.Trim().ToLower(CultureInfo.CurrentCulture);

        var tag = await this.tagRepository.GetTagByNameAsync(normalizedTagName);

        if (tag == null)
        {
            tag = await this.tagRepository.CreateTagAsync(normalizedTagName);
        }

        var taskTag = await this.tagRepository.GetTaskTagAsync(taskId, tag.Id);

        if (taskTag == null)
        {
            await this.tagRepository.AddTagToTaskAsync(taskId, tag.Id);
        }

        string formattedTagName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(tag.Name);

        return new TagDto { Id = tag.Id, Name = formattedTagName };
    }
}

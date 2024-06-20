using System.Globalization;
using Microsoft.Extensions.Logging;
using TodoListApp.Services.Database.Repositories;
using TodoListApp.Services.Exceptions;
using TodoListApp.Services.Interfaces;
using TodoListApp.Services.Logging;
using TodoListApp.WebApi.Models.Tags;

namespace TodoListApp.Services.Services;
public class TagService : ITagService
{
    private readonly ITagRepository tagRepository;
    private readonly ILogger<TagService> logger;

    public TagService(ITagRepository tagRepository, ILogger<TagService> logger)
    {
        this.tagRepository = tagRepository;
        this.logger = logger;
    }

    public async Task<IEnumerable<TagDto>> GetAllTagsAsync()
    {
        var tags = await this.tagRepository.GetAllTagsAsync();

        if (!tags.Any())
        {
            TagLoggerMessages.TagNotFoundWhileGettingAllTags(this.logger);
        }

        TagLoggerMessages.TagsRetrieved(this.logger);
        return tags.Select(t => new TagDto
        {
            Id = t.Id,
            Name = t.Name,
        });
    }

    public async Task<bool> IsTagNameUniqueAsync(string tagName)
    {
        try
        {
            string normalizedTagName = tagName.Trim().ToLower(CultureInfo.CurrentCulture);
            var existingTag = await this.tagRepository.GetTagByNameAsync(normalizedTagName);
            return existingTag == null;
        }
        catch (Exception ex)
        {
            TagLoggerMessages.ErrorOccurredWhileCheckingTagName(this.logger, tagName, ex.Message, ex);
            throw new ServiceException("An error occurred while checking if the tag name is unique.", ex);
        }
    }

    public async Task<TagDto> AddTagToTaskAsync(string tagName, int taskId)
    {
        try
        {
            string normalizedTagName = tagName.Trim().ToLower(CultureInfo.CurrentCulture);
            var tag = await this.tagRepository.CreateTagAsync(normalizedTagName);

            if (tag == null)
            {
                TagLoggerMessages.AddTagToTaskLogWarningFailed(this.logger, tagName);
                throw new InvalidOperationException("Tag creation failed.");
            }

            var taskTag = await this.tagRepository.GetTaskTagAsync(taskId, tag.Id);

            if (taskTag == null)
            {
                await this.tagRepository.AddTagToTaskAsync(taskId, tag.Id);
                TagLoggerMessages.TagAddedToTask(this.logger, tagName, taskId);
            }

            string formattedTagName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(tag.Name);

            return new TagDto { Id = tag.Id, Name = formattedTagName };
        }
        catch (InvalidOperationException ioe)
        {
            throw new ServiceException("An invalid operation occurred while adding the tag to the task.", ioe);
        }
        catch (Exception ex)
        {
            TagLoggerMessages.UnexpectedErrorOccurredWhileAddingTag(this.logger, ex.Message, ex);
            throw new ServiceException("An error occurred while adding the tag to the task.", ex);
        }
    }

    public async Task<bool> DeleteTagAsync(int taskId, int tagId)
    {
        try
        {
            return await this.tagRepository.DeleteTagAsync(taskId, tagId);
        }
        catch (Exception ex)
        {
            TagLoggerMessages.UnexpectedErrorOccurredWhileDeletingTag(this.logger, ex.Message, ex);
            throw new ServiceException("An error occurred while processing your request.", ex);
        }
    }
}

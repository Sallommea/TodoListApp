using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.Services.Exceptions;
using TodoListApp.Services.Interfaces;
using TodoListApp.WebApi.Logging;
using TodoListApp.WebApi.Models.Tags;

namespace TodoListApp.WebApi.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TagController : ControllerBase
{
    private readonly ITagService tagService;
    private readonly ILogger<TagController> logger;

    public TagController(ITagService tagService, ILogger<TagController> logger)
    {
        this.tagService = tagService;
        this.logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TagDto>>> GetAllTags()
    {
        string? userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return this.Unauthorized();
        }

        try
        {
            var tags = await this.tagService.GetAllTagsAsync(userId);

            if (!tags.Any())
            {
                TagControllerLoggerMessages.TagNotFoundWhileGettingAllTags(this.logger);
            }
            else
            {
                TagControllerLoggerMessages.TagsRetrieved(this.logger);
            }

            return this.Ok(tags);
        }
        catch (InvalidOperationException ioe)
        {
            TagControllerLoggerMessages.UnexpectedErrorOccurredWhileGettingAllTags(this.logger, ioe.Message, ioe);
            return this.StatusCode(StatusCodes.Status500InternalServerError, "An invalid operation occured" + ioe.Message);
        }
        catch (Exception ex)
        {
            TagControllerLoggerMessages.UnexpectedErrorOccurredWhileGettingAllTags(this.logger, ex.Message, ex);
            throw;
        }
    }

    [HttpPost("AddTagToTask")]
    public async Task<IActionResult> AddTagToTask([FromQuery] string tagName, [FromQuery] int taskId)
    {
        if (string.IsNullOrWhiteSpace(tagName))
        {
            TagControllerLoggerMessages.AddTagToTaskLogWarningTagEmpty(this.logger);
            return this.BadRequest("Tag name cannot be empty.");
        }

        string? userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return this.Unauthorized();
        }

        try
        {
            var tagDto = await this.tagService.AddTagToTaskAsync(tagName, taskId, userId);
            TagControllerLoggerMessages.TagAddedToTask(this.logger, tagName, taskId);
            return this.Ok(tagDto);
        }
        catch (ServiceException se)
        {
            TagControllerLoggerMessages.ServiceExceptionOccurredWhileAddingTag(this.logger, se.Message, se);
            return this.StatusCode(StatusCodes.Status500InternalServerError, se.Message);
        }
        catch (Exception ex)
        {
            TagControllerLoggerMessages.UnexpectedErrorOccurredWhileAddingTag(this.logger, ex.Message, ex);
            throw;
        }
    }

    [HttpDelete("{taskId}/tags/{tagId}")]
    public async Task<IActionResult> DeleteTag(int taskId, int tagId)
    {
        string? userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return this.Unauthorized();
        }

        try
        {
            if (taskId <= 0 || tagId <= 0)
            {
                return this.BadRequest("Invalid taskId or tagId.");
            }

            var result = await this.tagService.DeleteTagAsync(taskId, tagId, userId);

            if (result)
            {
                return this.NoContent();
            }

            return this.NotFound();
        }
        catch (KeyNotFoundException)
        {
            return this.NotFound();
        }
        catch (ServiceException se)
        {
            return this.StatusCode(StatusCodes.Status500InternalServerError, se.Message);
        }
        catch (Exception ex)
        {
            TagControllerLoggerMessages.UnexpectedErrorOccurredWhileDeletingTag(this.logger, ex.Message, ex);
            throw;
        }
    }
}

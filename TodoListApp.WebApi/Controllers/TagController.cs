using Microsoft.AspNetCore.Mvc;
using TodoListApp.Services.Interfaces;

namespace TodoListApp.WebApi.Controllers;
[Route("api/[controller]")]
[ApiController]
public class TagController : ControllerBase
{
    private readonly ITagService tagService;

    public TagController(ITagService tagService)
    {
        this.tagService = tagService;
    }

    [HttpPost("AddTagToTask")]
    public async Task<IActionResult> AddTagToTask([FromQuery] string tagName, [FromQuery] int taskId)
    {
        if (string.IsNullOrWhiteSpace(tagName))
        {
            return this.BadRequest("Tag name cannot be empty.");
        }

        try
        {
            var tagDto = await this.tagService.AddTagToTaskAsync(tagName, taskId);
            return this.Ok(tagDto);
        }
        catch (InvalidOperationException ioe)
        {
            return this.StatusCode(StatusCodes.Status500InternalServerError, "An invalid operation occured" + ioe.Message);
        }
    }
}

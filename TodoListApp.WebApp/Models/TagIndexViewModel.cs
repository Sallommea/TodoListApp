using TodoListApp.WebApi.Models.Tags;
using TodoListApp.WebApi.Models.Tasks;

namespace TodoListApp.WebApp.Models;

public class TagIndexViewModel
{
    public IEnumerable<TagDto> Tags { get; set; } = Enumerable.Empty<TagDto>();

    public List<TaskDto> Tasks { get; set; } = new List<TaskDto>();

    public int? SelectedTagId { get; set; }

    public int TotalPages { get; set; }

    public int CurrentPage { get; set; }

    public int TotalRecord { get; set; }

    public int PageSize { get; set; }
}

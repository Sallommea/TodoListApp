using System.ComponentModel.DataAnnotations;

namespace TodoListApp.WebApi.Models;
public class TodoListDto
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public int TaskCount { get; set; }
}

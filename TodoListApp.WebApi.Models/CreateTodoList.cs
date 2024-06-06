using System.ComponentModel.DataAnnotations;

namespace TodoListApp.WebApi.Models;
public class CreateTodoList
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;
}

using System.ComponentModel.DataAnnotations;

namespace TodoListApp.WebApi.Models.Tasks;
public class UpdateTaskStatus
{
    [Required(ErrorMessage = "Task ID is required.")]
    public int TaskId { get; set; }

    [Required(ErrorMessage = "Status is required.")]
    public Status Status { get; set; }
}

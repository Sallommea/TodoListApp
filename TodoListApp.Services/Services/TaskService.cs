using Microsoft.Extensions.Logging;
using TodoListApp.Services.Database;
using TodoListApp.Services.Database.Repositories;
using TodoListApp.Services.Exceptions;
using TodoListApp.Services.Interfaces;
using TodoListApp.Services.Logging;
using TodoListApp.Services.Models;
using TodoListApp.WebApi.Models.Comments;
using TodoListApp.WebApi.Models.Tags;
using TodoListApp.WebApi.Models.Tasks;

namespace TodoListApp.Services.Services;
public class TaskService : ITaskService
{
    private readonly ITaskRepository taskRepository;
    private readonly ILogger<TaskService> logger;

    public TaskService(ITaskRepository taskRepository, ILogger<TaskService> logger)
    {
        this.taskRepository = taskRepository;
        this.logger = logger;
    }

    public async Task<TaskDetailsDto?> GetTaskDetailsAsync(int taskId)
    {
        try
        {
            var task = await this.taskRepository.GetTaskByIdAsync(taskId);
            if (task == null)
            {
                TaskLoggerMessages.TaskIdNotFoundToGetTaskDetails(this.logger, taskId);
                throw new TaskException($"Task with ID {taskId} not found.");
            }

            var taskDetailsDto = new TaskDetailsDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                CreatedDate = task.CreatedDate,
                DueDate = task.DueDate,
                Status = (WebApi.Models.Tasks.Status)task.Status,
                Assignee = task.Assignee,
                TodoListId = task.TodoListId,
                IsExpired = task.IsExpired,
                Tags = task.TaskTags?.Select(tt => new TagDto
                {
                    Id = tt.Tag.Id,
                    Name = tt.Tag.Name,
                }).ToList() ?? new List<TagDto>(),
                Comments = task.Comments?.Select(c => new CommentDto
                {
                    Id = c.Id,
                    Content = c.Content,
                    CreatedDate = c.CreatedDate,
                    UserId = c.UserId,
                }).ToList() ?? new List<CommentDto>(),
            };

            return taskDetailsDto;
        }
        catch (TaskException)
        {
            throw;
        }
        catch (Exception ex)
        {
            TaskLoggerMessages.UnexpectedErrorOccurredWhileGettingTaskDetails(this.logger, ex.Message, ex);
            throw;
        }
    }

    public async Task<PaginatedListResult<TaskDto>> GetTasksByTagIdAsync(int tagId, int pageNumber, int pageSize)
    {
        try
        {
            var tasks = await this.taskRepository.GetTasksByTagIdAsync(tagId, pageNumber, pageSize);

            var taskDtos = (tasks.ResultList ?? new List<TaskEntity>()).Select(t => new TaskDto
            {
                Id = t.Id,
                Title = t.Title,
                DueDate = t.DueDate,
                Status = (WebApi.Models.Tasks.Status)t.Status,
                IsExpired = t.IsExpired,
            }).ToList();

            TaskLoggerMessages.TasksByTagIdRetrieved(this.logger, tagId);
            return new PaginatedListResult<TaskDto>
            {
                ResultList = taskDtos,
                TotalPages = tasks.TotalPages,
                TotalRecords = tasks.TotalRecords,
            };
        }
        catch (Exception ex)
        {
            TaskLoggerMessages.UnexpectedErrorgGettingTasksbyTagId(this.logger, tagId, ex.Message, ex);
            throw;
        }
    }

    public async Task<TaskDetailsDto> CreateTaskAsync(CreateTaskDto createTaskDto)
    {
        var currentDate = DateTime.UtcNow;

        var task = new TaskEntity
        {
            Title = createTaskDto.Title,
            Description = createTaskDto.Description,
            CreatedDate = DateTime.UtcNow,
            DueDate = createTaskDto.DueDate,
            Status = Database.Status.NotStarted,
            Assignee = "DefaultUser",
            TodoListId = createTaskDto.TodoListId,
            IsExpired = createTaskDto.DueDate.HasValue && createTaskDto.DueDate.Value < currentDate,
        };

        try
        {
            var createdTask = await this.taskRepository.AddTaskAsync(task);

            var taskDto = new TaskDetailsDto
            {
                Id = createdTask.Id,
                Title = createdTask.Title,
                Description = createdTask.Description,
                CreatedDate = createdTask.CreatedDate,
                DueDate = createdTask.DueDate,
                Status = (WebApi.Models.Tasks.Status)createdTask.Status,
                Assignee = createdTask.Assignee,
                TodoListId = createdTask.TodoListId,
                IsExpired = createdTask.IsExpired,
            };

            TaskLoggerMessages.TaskCreated(this.logger, createdTask.Id);
            return taskDto;
        }
        catch (KeyNotFoundException ex)
        {
            TaskLoggerMessages.ToDoListNotFound(this.logger, ex.Message);
            throw new TodoListException("Unable to create task due to missing TodoList.", ex);
        }
        catch (Exception ex)
        {
            TaskLoggerMessages.UnexpectedErrorCreatingTask(this.logger, ex);
            throw new TaskException("An unexpected error occurred while creating the task.", ex);
        }
    }

    public async Task<bool> DeleteTaskAsync(int taskId)
    {
        try
        {
            var result = await this.taskRepository.DeleteTaskAsync(taskId);
            if (!result)
            {
                TaskLoggerMessages.TaskIdNotFoundToDelete(this.logger, taskId);
                return false;
            }

            TaskLoggerMessages.TaskDeletedSuccessfully(this.logger, taskId);
            return true;
        }
        catch (Exception ex)
        {
            TaskLoggerMessages.UnexpectedErrorOccurredWhileDeletingTask(this.logger, ex.Message, taskId, ex);
            throw;
        }
    }

    public async Task<bool> UpdateTaskAsync(int taskId, UpdateTaskDto updateTaskDto)
    {
        try
        {
            var existingTask = await this.taskRepository.GetTaskByIdAsync(taskId);
            if (existingTask == null)
            {
                return false;
            }

            existingTask.Title = updateTaskDto.Title;
            existingTask.Description = updateTaskDto.Description;
            existingTask.DueDate = updateTaskDto.DueDate;
            existingTask.Status = (Database.Status)updateTaskDto.Status;

            var currentDateTime = DateTime.UtcNow;
            existingTask.IsExpired = updateTaskDto.DueDate.HasValue && updateTaskDto.DueDate.Value < currentDateTime;

            await this.taskRepository.UpdateTaskAsync(taskId, existingTask);
            return true;
        }
        catch (KeyNotFoundException)
        {
            TaskLoggerMessages.TaskIdNotFoundForUpdate(this.logger, taskId);
            return false;
        }
        catch (Exception ex)
        {
            TaskLoggerMessages.UnexpectedErrorOccurredWhileUpdatingTask(this.logger, ex.Message, taskId, ex);
            throw;
        }
    }

    public async Task<PaginatedListResult<TaskSearchResultDto>> GetPaginatedSearchedTasksAsync(int pageNumber, int itemsPerPage, string searchText)
    {
        try
        {
            var tasks = await this.taskRepository.SearchTasksByTitleAsync(pageNumber, itemsPerPage, searchText);
            var tasksSearched = (tasks.ResultList ?? new List<TaskEntity>()).Select(t => new TaskSearchResultDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                DueDate = t.DueDate,
                IsExpired = t.IsExpired,
                Status = (WebApi.Models.Tasks.Status)t.Status,
            }).ToList();

            var result = new PaginatedListResult<TaskSearchResultDto>
            {
                TotalRecords = tasks.TotalRecords,
                TotalPages = tasks.TotalPages,
                ResultList = tasksSearched,
            };
            TaskLoggerMessages.SearchedTasksRetrieved(this.logger);
            return result;
        }
        catch (Exception ex)
        {
            TaskLoggerMessages.UnexpectedErrorOccurredWhileSearchingTasks(this.logger, ex.Message, searchText, ex);
            throw;
        }
    }

    public async Task<CommentDto> AddCommentAsync(AddCommentDto addCommentDto)
    {
        try
        {
            var task = await this.taskRepository.GetTaskByIdAsync(addCommentDto.TaskId);
            if (task == null)
            {
                TaskLoggerMessages.TaskIdNotFoundForAddingComment(this.logger, addCommentDto.TaskId);
                throw new TaskException($"Task with ID {addCommentDto.TaskId} not found.");
            }

            var comment = new CommentEntity
            {
                TaskId = addCommentDto.TaskId,
                Content = addCommentDto.Content,
                CreatedDate = DateTime.UtcNow,
                UserName = addCommentDto.UserName ?? "Anonymous",
            };

            await this.taskRepository.AddCommentAsync(comment);

            return new CommentDto
            {
                Id = comment.Id,
                Content = comment.Content,
                CreatedDate = comment.CreatedDate,
                UserName = comment.UserName,
            };
        }
        catch (TaskException)
        {
            throw;
        }
        catch (Exception ex)
        {
            TaskLoggerMessages.UnexpectedErrorAddingCommentOnTask(this.logger, ex.Message, ex);
            throw new TaskException("An error occurred while adding the comment.", ex);
        }
    }

    public async Task<CommentDto> EditCommentAsync(EditCommentDto editCommentDto)
    {
        try
        {
            var comment = await this.taskRepository.GetCommentByIdAsync(editCommentDto.CommentId);
            if (comment == null)
            {
                TaskLoggerMessages.CommentIdNotFoundForEditingComment(this.logger, editCommentDto.CommentId);
                throw new TaskException($"Comment with ID {editCommentDto.CommentId} not found.");
            }

            if (comment.TaskId != editCommentDto.TaskId)
            {
                TaskLoggerMessages.CommentIdDoNotBelongToTaskEdit(this.logger, editCommentDto.CommentId, editCommentDto.TaskId);
                throw new TaskException($"Comment does not belong to the specified task.");
            }

            comment.Content = editCommentDto.Content;

            await this.taskRepository.UpdateCommentAsync(comment);

            return new CommentDto
            {
                Id = comment.Id,
                Content = comment.Content,
                CreatedDate = comment.CreatedDate,
                UserName = comment.UserName,
            };
        }
        catch (TaskException)
        {
            throw;
        }
        catch (Exception ex)
        {
            TaskLoggerMessages.UnexpectedErrorEditingCommentOnTask(this.logger, ex.Message, ex);
            throw new TaskException("An error occurred while editing the comment.", ex);
        }
    }

    public async Task DeleteCommentAsync(int taskId, int commentId)
    {
        try
        {
            var comment = await this.taskRepository.GetCommentByIdAsync(commentId);
            if (comment == null)
            {
                TaskLoggerMessages.CommentIdNotFoundForDeletingComment(this.logger, commentId);
                throw new TaskException($"Comment with ID {commentId} not found.");
            }

            if (comment.TaskId != taskId)
            {
                TaskLoggerMessages.CommentIdDoNotBelongToTaskDelete(this.logger, commentId, taskId);
                throw new TaskException($"Comment does not belong to the specified task.");
            }

            await this.taskRepository.DeleteCommentAsync(comment);
        }
        catch (TaskException)
        {
            throw;
        }
        catch (Exception ex)
        {
            TaskLoggerMessages.UnexpectedErrorDeletingCommentOnTask(this.logger, ex.Message, ex);
            throw new TaskException("An error occurred while deleting the comment.", ex);
        }
    }
}

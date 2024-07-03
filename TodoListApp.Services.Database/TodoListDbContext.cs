using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TodoListApp.Services.Database;
public class TodoListDbContext : IdentityDbContext<User>
{
    public TodoListDbContext(DbContextOptions<TodoListDbContext> options)
           : base(options)
    {
    }

    public DbSet<TodoListEntity> TodoLists { get; set; }

    public DbSet<TaskEntity> Tasks { get; set; }

    public DbSet<TagEntity> Tags { get; set; }

    public DbSet<TaskTagEntity> TaskTags { get; set; }

    public DbSet<CommentEntity> Comments { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        _ = builder.Entity<TodoListEntity>(entity =>
        {
            _ = entity.HasKey(e => e.Id);
            _ = entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            _ = entity.Property(e => e.Description).HasMaxLength(500);

            _ = entity.HasMany(e => e.Tasks)
                .WithOne(t => t.TodoList)
                .HasForeignKey(t => t.TodoListId)
                .OnDelete(DeleteBehavior.Cascade);

            _ = entity.HasOne(e => e.User)
              .WithMany(u => u.Todos)
              .HasForeignKey(e => e.UserId)
              .OnDelete(DeleteBehavior.Restrict);
        });

        _ = builder.Entity<TaskEntity>(entity =>
        {
            _ = entity.HasKey(e => e.Id);
            _ = entity.Property(e => e.Title).IsRequired().HasMaxLength(100);
            _ = entity.Property(e => e.Description).HasMaxLength(500);
            _ = entity.Property(e => e.CreatedDate).IsRequired();
            _ = entity.Property(e => e.Status).IsRequired();

            _ = entity.HasOne(e => e.TodoList)
                .WithMany(t => t.Tasks)
                .HasForeignKey(e => e.TodoListId)
                .OnDelete(DeleteBehavior.Cascade);

            _ = entity.HasMany(t => t.TaskTags)
                .WithOne(tt => tt.Task)
                .HasForeignKey(tt => tt.TaskId)
                .IsRequired(false);

            _ = entity.HasOne(e => e.User)
              .WithMany()
              .HasForeignKey(e => e.UserId)
              .OnDelete(DeleteBehavior.Restrict);
        });

        _ = builder.Entity<TagEntity>(entity =>
        {
            _ = entity.HasKey(e => e.Id);
            _ = entity.Property(e => e.Name).IsRequired().HasMaxLength(50);

            _ = entity.HasMany(t => t.TaskTags)
                .WithOne(tt => tt.Tag)
                .HasForeignKey(tt => tt.TagId)
                .IsRequired();

            _ = entity.HasOne(e => e.User)
              .WithMany()
              .HasForeignKey(e => e.UserId)
              .OnDelete(DeleteBehavior.Restrict);
        });

        _ = builder.Entity<TaskTagEntity>(entity =>
        {
            _ = entity.HasKey(tt => new { tt.TaskId, tt.TagId });

            _ = entity.HasOne(tt => tt.Task)
                .WithMany(t => t.TaskTags)
                .HasForeignKey(tt => tt.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            _ = entity.HasOne(tt => tt.Tag)
                .WithMany(t => t.TaskTags)
                .HasForeignKey(tt => tt.TagId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        _ = builder.Entity<CommentEntity>(entity =>
        {
            _ = entity.HasKey(e => e.Id);
            _ = entity.Property(e => e.Content).IsRequired().HasMaxLength(1500);
            _ = entity.Property(e => e.CreatedDate).IsRequired();
            _ = entity.Property(e => e.UserId).IsRequired(false);
            _ = entity.HasOne(e => e.Task)
                .WithMany(t => t.Comments)
                .HasForeignKey(e => e.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            _ = entity.HasOne(e => e.User)
              .WithMany()
              .HasForeignKey(e => e.UserId)
              .OnDelete(DeleteBehavior.Restrict);
        });
    }
}

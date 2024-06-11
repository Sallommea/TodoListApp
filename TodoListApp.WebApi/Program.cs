using Microsoft.EntityFrameworkCore;
using TodoListApp.Services.Database;
using TodoListApp.Services.Database.Repositories;
using TodoListApp.Services.Interfaces;
using TodoListApp.Services.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(loggingBuilder =>
{
    _ = loggingBuilder.AddConsole();
    _ = loggingBuilder.AddDebug();
});

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddDbContext<TodoListDbContext>(opts =>
{
    _ = opts.UseSqlServer(builder.Configuration["ConnectionStrings:TodoListDbConnection"]);
});

builder.Services.AddScoped<ITodoListRepository, TodolistRepository>();
builder.Services.AddScoped<ITodoListService, TodoListService>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IAssignedTasksService, AssignedTasksService>();
builder.Services.AddScoped<IAssignedTasksRepository, AssignedTasksRepository>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

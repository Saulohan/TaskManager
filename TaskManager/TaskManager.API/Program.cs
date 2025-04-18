using TaskManager.Database;
using TaskManager.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using TaskManager.Domain.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IPasswordHasher<TaskItem>, PasswordHasher<TaskItem>>();
builder.Services.AddScoped<IPasswordHasher<Project>, PasswordHasher<Project>>();

builder.Services.AddDbContext<TaskManagerContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("TaskManagerConnectionString")));

builder.Services.AddScoped<Repository>();
builder.Services.AddScoped<TaskItemRepository>();
builder.Services.AddScoped<TaskItemHistoricRepository>();
builder.Services.AddScoped<TaskCommentRepository>();
builder.Services.AddScoped<ProjectRepository>();

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options => options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

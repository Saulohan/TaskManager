using Microsoft.EntityFrameworkCore;
using TaskManager.Database;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure;

public partial class TaskCommentRepository(TaskManagerContext context)
{
    public async Task Save(TaskComment taskComment)
    {
        if (taskComment.Id is null)
            await AddtaskItemHistoric(taskComment);
        else
            await UpdatetaskItemHistoric(taskComment);

        await context.SaveChangesAsync();
    }

    public async Task AddtaskItemHistoric(TaskComment taskComment)
    {
        await context.TaskComment.AddAsync(taskComment);
    } 

    public async Task UpdatetaskItemHistoric(TaskComment taskComment)
    {
        context.TaskComment.Update(taskComment);
    }

    public async Task<List<TaskComment>> GetAllTaskCommentByTaskItemId(long taskItemId) => await context.TaskComment.Where(x => x.DeletedAt == null && x.TaskItem.Id.Value == taskItemId).ToListAsync();

}
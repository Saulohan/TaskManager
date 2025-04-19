using TaskManager.Database;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure;

public  partial class Repository(TaskManagerContext context)
{
    public virtual async Task AddAsync(User user)
    {
        await context.User.AddAsync(user);
    }

    public virtual async void Save()
    {
        await context.SaveChangesAsync();
    }

}
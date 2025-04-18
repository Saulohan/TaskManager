using TaskManager.Database;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure;

public partial class Repository(TaskManagerContext context)
{
    public async Task AddAsync(User user)
    {
        await context.User.AddAsync(user);
    }

    public async void Save()
    {
        await context.SaveChangesAsync();
    }

    // Os métodos estão encapsulados nos seus proprios arquivos.
}
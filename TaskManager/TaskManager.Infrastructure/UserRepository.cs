using Microsoft.EntityFrameworkCore;
using TaskManager.Database;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure;

public partial class UserRepository(TaskManagerContext context)
{
    public virtual async Task<List<User>> GetAllUsers() => await context.User.Select(x => x).Where(x => x.DeletedAt == null).ToListAsync();
    
    public virtual async Task<User> GetUserById(long userId) => await context.User.Where(x => x.DeletedAt == null && x.Id == userId).FirstOrDefaultAsync();
}
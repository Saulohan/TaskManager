using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure;

public partial class Repository
{
    //validar necessidade
    public async Task<List<User>> GetAllUsers() => await context.User.Select(x => x).Where(x => x.DeletedAt == null).ToListAsync();

    
    public async Task<bool> UserExistsAsync(string username) => await context.User.AnyAsync(user => user.Username == username && user.DeletedAt == null);
    
}
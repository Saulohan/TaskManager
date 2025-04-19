using TaskManager.Models.Enums;

namespace TaskManager.Domain.Entities;

public class User : BaseModel
{
    public string Username { get; set; }

    public UserType Type { get; set; }
}
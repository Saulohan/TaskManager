using TaskManager.Models.Enums;

namespace TaskManager.Domain.Entities;

public class User : BaseModel
{

    public string Username { get; set; }

    public UserType Type { get; set; }

    //public long UserId { get; set; }
    //public override long GetId() => UserId;

    //public override void SetId(long id) => UserId = id;
}
namespace TaskManager.Domain.Entities;

public abstract class BaseModel
{
    public long? Id { get; set; }

    public virtual DateTimeOffset CreatedAt { get; set; }

    public virtual DateTimeOffset? UpdatedAt { get; set; }

    public virtual DateTimeOffset? DeletedAt { get; set; }

    public long CreatedBy { get; set; }

    public long? UpdatedBy { get; set; }

    public long GetId() => Id.Value;

    public void SetId(long id) => Id = id;
}
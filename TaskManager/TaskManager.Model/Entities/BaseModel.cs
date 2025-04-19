namespace TaskManager.Domain.Entities;

public class BaseModel
{
    protected BaseModel(){  }
    protected BaseModel(long id){ Id = id; }

    public long? Id { get; set; }

    public virtual DateTime CreatedAt { get; set; }

    public virtual DateTime? UpdatedAt { get; set; }

    public virtual DateTime? DeletedAt { get; set; }

    public long CreatedBy { get; set; }

    public long? UpdatedBy { get; set; }

    public long GetId() => Id.Value;

    public void SetId(long id) => Id = id;
}
using System;

namespace RachaConta.Core.Entities;

public class Category
{
    public Guid Id { get; private set; }
    public string Description { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Category(string description, bool isActive = true)
    {
        Id = Guid.NewGuid();
        Description = description;
        IsActive = isActive;
        CreatedAt = DateTime.UtcNow;
    }

    // EF Core constructor
    protected Category() { }

    public void Update(string description, bool isActive)
    {
        Description = description;
        IsActive = isActive;
    }
}

using System;

namespace RachaConta.Application.DTOs.Response;

public class CategoryResponse 
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

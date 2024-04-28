using StoreManagement.Bases;

namespace StoreManagement.Domain.Entities;

public class Category : BaseEntity<Guid>
{    
    public string Name { get; set; }  = string.Empty;
    public string? Description { get; set; }
    public string? Photo { get; set; }
}

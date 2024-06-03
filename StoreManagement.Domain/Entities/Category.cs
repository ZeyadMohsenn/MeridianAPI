using StoreManagement.Bases;
using System.ComponentModel.DataAnnotations;

namespace StoreManagement.Domain.Entities;

public class Category : BaseEntity<Guid>
{
    [MinLength(3), MaxLength(25)]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? StoredFileName { get; set; }
    public ICollection<SubCategory> SubCategories { get; set; } = [];

}

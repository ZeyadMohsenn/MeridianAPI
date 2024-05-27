using Microsoft.AspNetCore.Identity;
using StoreManagement.Bases;

namespace StoreManagement.Domain.Entities;

public class Cashier : BaseEntity<Guid>
{
    public virtual ApplicationUser User { get; set; }
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    public string UserName { get; set; }
    
}

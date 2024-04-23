using Microsoft.AspNetCore.Identity;

namespace StoreManagement.Domain.Entities;

public class ApplicationRole : IdentityRole<Guid>
{
    public bool Is_Active { get; set; }
    public Guid Created_By { get; set; }
    public DateTime Creation_Time { get; set; }
    public Guid? Last_Modify_By { get; set; }
    public DateTime? Last_Modify_Time { get; set; }
    public bool Is_Deleted { get; set; }
    public virtual ICollection<ApplicationUserRole> UserRoles { get; set; } = new HashSet<ApplicationUserRole>();
}


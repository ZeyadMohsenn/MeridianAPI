using Microsoft.AspNetCore.Identity;

namespace StoreManagement.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public string Full_Name { get; set; }
    public bool Is_Active { get; set; } = true;
    public bool Is_Deleted { get; set; }
    public DateTime? Last_Login_Time { get; set; }
    public DateTime Creation_Time { get; set; }
    public Guid Created_By { get; set; }
    public DateTime? Last_Modify_Time { get; set; }
    public Guid? Last_Modify_By { get; set; }
    public string Email_Token { get; set; } = "";
    public bool Token_Revoked { get; set; }
    public virtual ICollection<ApplicationUserRole> UserRoles { get; set; } = [];

}


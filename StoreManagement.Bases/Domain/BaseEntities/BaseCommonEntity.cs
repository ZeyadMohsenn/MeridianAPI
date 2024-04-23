using System;

namespace StoreManagement.Bases;

public class BaseCommonEntity<IdType> : BaseEntity<IdType>
{
    public Guid Created_By { get; set; }
    public DateTime Creation_Time { get; set; }
    public Guid? Last_Modify_By { get; set; }
    public DateTime? Last_Modify_Time { get; set; }
}

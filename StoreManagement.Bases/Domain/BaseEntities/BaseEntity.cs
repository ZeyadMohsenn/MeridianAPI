using System.ComponentModel.DataAnnotations.Schema;

namespace StoreManagement.Bases;

public class BaseEntity<IdType>
{
    public IdType Id { get; set; }
    public bool Is_Deleted { get; set; }

}


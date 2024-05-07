using System;

namespace StoreManagement.Bases.Domain.Model;

public class GetAllSubCategoriesFilter : PagingModel
{
    public string SubCategoryName { get; set; }
    public bool? Is_Deleted { get; set; }
    public Guid CategoryId { get; set; }
}

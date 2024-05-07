namespace StoreManagement.Bases.Domain.Model
{
    public class GetAllCategoriesFilter : PagingModel
    {
        public string CategoryName { get; set; } = string.Empty;
        public bool? Is_Deleted { get; set; }

    }
}

namespace StoreManagement.Domain
{
    public class PaginationResponse<T>
    {
        public int Length { get; set; }
        public List<T> Collection { get; set; }
    }
}

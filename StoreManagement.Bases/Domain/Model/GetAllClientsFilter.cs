namespace StoreManagement.Bases.Domain.Model
{
    public class GetAllClientsFilter : PagingModel
    {
        public string Name { get; set; }
        public string Phone { get; set; }

    }
}

namespace StoreManagement.Domain.Dtos.Reports;

public class OrderStatsDto
{
    public int OrdersCount { get; set; }
    public decimal TotalDiscount { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal TotalRemained { get; set; }
    public int ClientsCount { get; set; }
    public List<DailyOrderDto> DailyOrders { get; set; }
}

public class DailyOrderDto
{
    public DateTime Date { get; set; }
    public List<OrderDto> Orders { get; set; }
}

public class OrderDto
{
    public String? Status { get; set; }
    public Guid OrderId { get; set; }
    public string ClientName { get; set; }
    public DateTime CraetedAt { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal TotalDiscount { get; set; }
    public decimal Paid { get; set; }
    public decimal Remained { get; set; }

}

namespace WebStore.ViewModels.VM;

public class OrderVm
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime DeliveryDate { get; set; }
    public decimal OrderAmount { get; set; }
    public long TrackingNumber { get; set; }
    public int CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public int? InvoiceId { get; set; }
    public List<OrderProductVm> OrderProducts { get; set; } = new();
}

public class OrderProductVm
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = default!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}


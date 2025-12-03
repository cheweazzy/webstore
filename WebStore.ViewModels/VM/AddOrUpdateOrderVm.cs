using System.ComponentModel.DataAnnotations;

namespace WebStore.ViewModels.VM;

public class AddOrUpdateOrderVm
{
    public int? Id { get; set; }
    [Required]
    public DateTime OrderDate { get; set; }
    [Required]
    public DateTime DeliveryDate { get; set; }
    [Required]
    public decimal OrderAmount { get; set; }
    [Required]
    public long TrackingNumber { get; set; }
    [Required]
    public int CustomerId { get; set; }
    public List<OrderProductItemVm> OrderProducts { get; set; } = new();
}

public class OrderProductItemVm
{
    [Required]
    public int ProductId { get; set; }
    [Required]
    public int Quantity { get; set; }
}


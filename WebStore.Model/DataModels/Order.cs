namespace WebStore.Model.DataModels;

public class Order {
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime DeliveryDate { get; set; }
    public decimal OrderAmount { get; set; }
    public long TrackingNumber { get; set; }
    public Customer Customer { get; set; }
    public int CustomerId { get; set; }
    public Invoice? Invoice { get; set; }
    public IList<OrderProduct> OrderProducts { get; set; }
}

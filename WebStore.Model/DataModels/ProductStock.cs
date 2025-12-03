namespace WebStore.Model.DataModels;

public class ProductStock {
    public int Id { get; set; }
    public Product Product { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
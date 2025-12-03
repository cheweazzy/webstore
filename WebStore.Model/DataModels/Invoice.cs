namespace WebStore.Model.DataModels;

public class Invoice {
    public int Id { get; set; }
    public string InvoiceNumber { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public decimal Amount { get; set; }
    public Order Order { get; set; }
    public int OrderId { get; set; }
}


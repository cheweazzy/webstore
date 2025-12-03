namespace WebStore.ViewModels.VM;

public class InvoiceVm
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = default!;
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public decimal Amount { get; set; }
    public int OrderId { get; set; }
    public long? OrderTrackingNumber { get; set; }
}


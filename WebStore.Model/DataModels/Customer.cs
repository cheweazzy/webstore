using Microsoft.AspNetCore.Identity;

namespace WebStore.Model.DataModels;

public class Customer : User {
    public Address BillingAddress { get; set; }
    public IList<Order> Orders { get; set; }
    public Address ShippingAddress { get; set; }
}
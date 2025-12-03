using Microsoft.AspNetCore.Identity;

namespace WebStore.Model.DataModels;

public class Supplier : User {
    public string CompanyName { get; set; }
    public IList<Product> Products { get; set; }
}
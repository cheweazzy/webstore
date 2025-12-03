using System.ComponentModel.DataAnnotations;

namespace WebStore.ViewModels.VM;

public class AddOrUpdateAddressVm
{
    [Required]
    public string Street { get; set; } = default!;
    [Required]
    public string City { get; set; } = default!;
    [Required]
    public string PostalCode { get; set; } = default!;
    [Required]
    public string Country { get; set; } = default!;
}


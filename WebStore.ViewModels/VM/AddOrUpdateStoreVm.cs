using System.ComponentModel.DataAnnotations;
using WebStore.Model.DataModels;

namespace WebStore.ViewModels.VM;

public class AddOrUpdateStoreVm
{
    public int? Id { get; set; }
    [Required]
    public string Name { get; set; } = default!;
    [Required]
    public AddressVm Address { get; set; } = default!;
}


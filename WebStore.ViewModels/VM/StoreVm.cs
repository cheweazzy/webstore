using WebStore.Model.DataModels;

namespace WebStore.ViewModels.VM;

public class StoreVm
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public AddressVm Address { get; set; } = default!;
    public int EmployeeCount { get; set; }
}


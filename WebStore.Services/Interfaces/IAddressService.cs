using WebStore.Model.DataModels;
using WebStore.ViewModels.VM;

namespace WebStore.Services.Interfaces;

public interface IAddressService
{
    AddressVm ValidateAddress(AddOrUpdateAddressVm addressVm);
    bool CompareAddresses(Address address1, Address address2);
    string FormatAddress(AddressVm addressVm);
}


using System;
using Microsoft.Extensions.Logging;
using WebStore.DAL.EF;
using WebStore.Model.DataModels;
using WebStore.Services.Interfaces;
using WebStore.ViewModels.VM;

namespace WebStore.Services.ConcreteServices;

public class AddressService : BaseService, IAddressService
{
    public AddressService(WebStoreDbContext dbContext, AutoMapper.IMapper mapper, ILogger<AddressService> logger)
        : base(dbContext, mapper, logger)
    {
    }

    public AddressVm ValidateAddress(AddOrUpdateAddressVm addressVm)
    {
        if (string.IsNullOrWhiteSpace(addressVm.Street))
        {
            throw new ArgumentException("Street is required.");
        }

        if (string.IsNullOrWhiteSpace(addressVm.City))
        {
            throw new ArgumentException("City is required.");
        }

        if (string.IsNullOrWhiteSpace(addressVm.PostalCode))
        {
            throw new ArgumentException("PostalCode is required.");
        }

        if (string.IsNullOrWhiteSpace(addressVm.Country))
        {
            throw new ArgumentException("Country is required.");
        }

        return new AddressVm
        {
            Street = addressVm.Street.Trim(),
            City = addressVm.City.Trim(),
            PostalCode = addressVm.PostalCode.Trim(),
            Country = addressVm.Country.Trim()
        };
    }

    public bool CompareAddresses(Address address1, Address address2)
    {
        if (address1 == null || address2 == null)
        {
            return false;
        }

        return string.Equals(address1.Street?.Trim(), address2.Street?.Trim(), StringComparison.OrdinalIgnoreCase) &&
               string.Equals(address1.City?.Trim(), address2.City?.Trim(), StringComparison.OrdinalIgnoreCase) &&
               string.Equals(address1.PostalCode?.Trim(), address2.PostalCode?.Trim(), StringComparison.OrdinalIgnoreCase) &&
               string.Equals(address1.Country?.Trim(), address2.Country?.Trim(), StringComparison.OrdinalIgnoreCase);
    }

    public string FormatAddress(AddressVm addressVm)
    {
        if (addressVm == null)
        {
            throw new ArgumentNullException(nameof(addressVm));
        }

        return $"{addressVm.Street}, {addressVm.PostalCode} {addressVm.City}, {addressVm.Country}";
    }
}


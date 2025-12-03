using System;
using Microsoft.Extensions.DependencyInjection;
using WebStore.DAL.EF;
using WebStore.Model.DataModels;
using WebStore.Services.Interfaces;
using WebStore.Tests.Fixtures;
using WebStore.ViewModels.VM;
using Xunit;

namespace WebStore.Tests.UnitTests;

public class AddressServiceUnitTests : IClassFixture<TestDataFixture>
{
    private readonly TestDataFixture _fixture;
    private readonly IAddressService _addressService;

    public AddressServiceUnitTests(TestDataFixture fixture)
    {
        _fixture = fixture;
        _addressService = fixture.ServiceProvider.GetRequiredService<IAddressService>();
    }

    [Fact]
    public void ValidateAddressTest()
    {
        var addressVm = new AddOrUpdateAddressVm
        {
            Street = "ul. Testowa 15",
            City = "Warszawa",
            PostalCode = "00-001",
            Country = "Poland"
        };

        var validatedAddress = _addressService.ValidateAddress(addressVm);
        Assert.NotNull(validatedAddress);
        Assert.Equal("ul. Testowa 15", validatedAddress.Street);
        Assert.Equal("Warszawa", validatedAddress.City);
    }

    [Fact]
    public void ValidateAddressWithEmptyStreetTest()
    {
        var addressVm = new AddOrUpdateAddressVm
        {
            Street = "",
            City = "Warszawa",
            PostalCode = "00-001",
            Country = "Poland"
        };

        Assert.Throws<ArgumentException>(() => _addressService.ValidateAddress(addressVm));
    }

    [Fact]
    public void CompareAddressesTest()
    {
        var address1 = new Address
        {
            Street = "ul. Testowa 15",
            City = "Warszawa",
            PostalCode = "00-001",
            Country = "Poland"
        };

        var address2 = new Address
        {
            Street = "ul. Testowa 15",
            City = "Warszawa",
            PostalCode = "00-001",
            Country = "Poland"
        };

        var result = _addressService.CompareAddresses(address1, address2);
        Assert.True(result);
    }

    [Fact]
    public void CompareDifferentAddressesTest()
    {
        var address1 = new Address
        {
            Street = "ul. Testowa 15",
            City = "Warszawa",
            PostalCode = "00-001",
            Country = "Poland"
        };

        var address2 = new Address
        {
            Street = "ul. Inna 20",
            City = "Kraków",
            PostalCode = "30-001",
            Country = "Poland"
        };

        var result = _addressService.CompareAddresses(address1, address2);
        Assert.False(result);
    }

    [Fact]
    public void FormatAddressTest()
    {
        var addressVm = new AddressVm
        {
            Street = "ul. Testowa 15",
            City = "Warszawa",
            PostalCode = "00-001",
            Country = "Poland"
        };

        var formatted = _addressService.FormatAddress(addressVm);
        Assert.Equal("ul. Testowa 15, 00-001 Warszawa, Poland", formatted);
    }
}


using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using WebStore.DAL.EF;
using WebStore.Model.DataModels;
using WebStore.Services.Interfaces;
using WebStore.Tests.Fixtures;
using WebStore.ViewModels.VM;
using Xunit;

namespace WebStore.Tests.UnitTests;

public class StoreServiceUnitTests : IClassFixture<TestDataFixture>
{
    private readonly TestDataFixture _fixture;
    private readonly IStoreService _storeService;
    private readonly WebStoreDbContext _dbContext;

    public StoreServiceUnitTests(TestDataFixture fixture)
    {
        _fixture = fixture;
        _storeService = fixture.ServiceProvider.GetRequiredService<IStoreService>();
        _dbContext = fixture.DbContext;
    }

    [Fact]
    public void GetStoreTest()
    {
        var store = _storeService.GetStore(s => s.Name == "Store Warszawa");
        Assert.NotNull(store);
        Assert.Equal("Store Warszawa", store.Name);
    }

    [Fact]
    public void GetMultipleStoresTest()
    {
        var stores = _storeService.GetStores(s => s.Id >= 1);
        Assert.NotNull(stores);
    }

    [Fact]
    public void GetAllStoresTest()
    {
        var stores = _storeService.GetStores();
        Assert.NotNull(stores);
    }

    [Fact]
    public void AddNewStoreTest()
    {
        var newStoreVm = new AddOrUpdateStoreVm()
        {
            Name = "Store Kraków",
            Address = new AddressVm
            {
                Street = "ul. Testowa 10",
                City = "Kraków",
                PostalCode = "30-001",
                Country = "Poland"
            }
        };

        var createdStore = _storeService.AddOrUpdateStore(newStoreVm);
        Assert.NotNull(createdStore);
        Assert.Equal("Store Kraków", createdStore.Name);
        Assert.Equal("Kraków", createdStore.Address.City);
    }

    [Fact]
    public void UpdateStoreTest()
    {
        // Get existing store
        var existingStore = _dbContext.StationaryStores.First();
        
        var updateStoreVm = new AddOrUpdateStoreVm()
        {
            Id = existingStore.Id,
            Name = "Store Warszawa Updated",
            Address = new AddressVm
            {
                Street = "ul. Nowa 20",
                City = "Warszawa",
                PostalCode = "00-001",
                Country = "Poland"
            }
        };

        var editedStore = _storeService.AddOrUpdateStore(updateStoreVm);
        Assert.NotNull(editedStore);
        Assert.Equal("Store Warszawa Updated", editedStore.Name);
        Assert.Equal("ul. Nowa 20", editedStore.Address.Street);
    }

    [Fact]
    public void DeleteStoreTest()
    {
        // Get existing store
        var existingStore = _dbContext.StationaryStores.First();
        var result = _storeService.DeleteStore(existingStore.Id);
        Assert.True(result);
    }
}


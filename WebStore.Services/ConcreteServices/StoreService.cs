using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebStore.DAL.EF;
using WebStore.Model.DataModels;
using WebStore.Services.Interfaces;
using WebStore.ViewModels.VM;

namespace WebStore.Services.ConcreteServices;

public class StoreService : BaseService, IStoreService
{
    public StoreService(WebStoreDbContext dbContext, AutoMapper.IMapper mapper, ILogger<StoreService> logger)
        : base(dbContext, mapper, logger)
    {
    }

    public StoreVm AddOrUpdateStore(AddOrUpdateStoreVm addOrUpdateStoreVm)
    {
        StationaryStore store;

        if (addOrUpdateStoreVm.Id.HasValue && addOrUpdateStoreVm.Id.Value > 0)
        {
            // Update existing store
            store = DbContext.StationaryStores
                .Include(s => s.Employees)
                .FirstOrDefault(s => s.Id == addOrUpdateStoreVm.Id.Value)
                ?? throw new ArgumentException($"Store with id {addOrUpdateStoreVm.Id.Value} not found.");

            store.Name = addOrUpdateStoreVm.Name;
            store.Address = new Address
            {
                Street = addOrUpdateStoreVm.Address.Street,
                City = addOrUpdateStoreVm.Address.City,
                PostalCode = addOrUpdateStoreVm.Address.PostalCode,
                Country = addOrUpdateStoreVm.Address.Country
            };
        }
        else
        {
            // Create new store
            store = new StationaryStore
            {
                Name = addOrUpdateStoreVm.Name,
                Address = new Address
                {
                    Street = addOrUpdateStoreVm.Address.Street,
                    City = addOrUpdateStoreVm.Address.City,
                    PostalCode = addOrUpdateStoreVm.Address.PostalCode,
                    Country = addOrUpdateStoreVm.Address.Country
                },
                Employees = new List<StationaryStoreEmployee>()
            };

            DbContext.StationaryStores.Add(store);
        }

        DbContext.SaveChanges();

        return MapToStoreVm(store);
    }

    public StoreVm GetStore(Expression<Func<StationaryStore, bool>> filterExpression)
    {
        var store = DbContext.StationaryStores
            .Include(s => s.Employees)
            .FirstOrDefault(filterExpression);

        if (store == null)
        {
            throw new ArgumentException("Store not found.");
        }

        return MapToStoreVm(store);
    }

    public IEnumerable<StoreVm> GetStores(Expression<Func<StationaryStore, bool>>? filterExpression = null)
    {
        IQueryable<StationaryStore> query = DbContext.StationaryStores
            .Include(s => s.Employees);

        if (filterExpression != null)
        {
            query = query.Where(filterExpression);
        }

        var stores = query.ToList();

        return stores.Select(MapToStoreVm);
    }

    public bool DeleteStore(int storeId)
    {
        var store = DbContext.StationaryStores.FirstOrDefault(s => s.Id == storeId);
        if (store == null)
        {
            return false;
        }

        DbContext.StationaryStores.Remove(store);
        DbContext.SaveChanges();
        return true;
    }

    private StoreVm MapToStoreVm(StationaryStore store)
    {
        return new StoreVm
        {
            Id = store.Id,
            Name = store.Name,
            Address = new AddressVm
            {
                Street = store.Address.Street,
                City = store.Address.City,
                PostalCode = store.Address.PostalCode,
                Country = store.Address.Country
            },
            EmployeeCount = store.Employees?.Count ?? 0
        };
    }
}


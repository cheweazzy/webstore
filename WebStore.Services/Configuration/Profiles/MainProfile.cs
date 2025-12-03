using AutoMapper;
using WebStore.Model.DataModels;
using WebStore.ViewModels.VM;

namespace WebStore.Services.Configuration.Profiles;

public class MainProfile : Profile
{
    public MainProfile()
    {
        // Product mappings
        CreateMap<Product, ProductVm>();
        CreateMap<AddOrUpdateProductVm, Product>();

        // Order mappings
        CreateMap<Order, OrderVm>();
        CreateMap<AddOrUpdateOrderVm, Order>();
        CreateMap<OrderProduct, OrderProductVm>();

        // Invoice mappings
        CreateMap<Invoice, InvoiceVm>();
        CreateMap<AddOrUpdateInvoiceVm, Invoice>();

        // Store mappings
        CreateMap<StationaryStore, StoreVm>();
        CreateMap<AddOrUpdateStoreVm, StationaryStore>();

        // Address mappings
        CreateMap<Address, AddressVm>();
        CreateMap<AddressVm, Address>();
        CreateMap<AddOrUpdateAddressVm, AddressVm>();
    }
}


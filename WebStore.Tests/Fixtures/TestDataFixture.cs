using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebStore.DAL.EF;
using WebStore.Model.DataModels;

namespace WebStore.Tests.Fixtures;

public class TestDataFixture : IDisposable, IAsyncLifetime
{
    public WebStoreDbContext DbContext { get; private set; }
    public UserManager<User> UserManager { get; private set; }
    public IServiceProvider ServiceProvider { get; private set; }

    public TestDataFixture()
    {
        var services = new ServiceCollection();
        
        services.AddAutoMapper(typeof(WebStore.Services.Configuration.Profiles.MainProfile));
        services.AddEntityFrameworkInMemoryDatabase()
            .AddDbContext<WebStoreDbContext>(options =>
                options.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}"));

        services.AddLogging();
        
        services.AddIdentity<User, IdentityRole<int>>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
            options.Password.RequiredLength = 6;
            options.Password.RequiredUniqueChars = 0;
            options.Password.RequireNonAlphanumeric = false;
        })
        .AddRoleManager<RoleManager<IdentityRole<int>>>()
        .AddUserManager<UserManager<User>>()
        .AddEntityFrameworkStores<WebStoreDbContext>();
        
        // Register services
        services.AddTransient<WebStore.Services.Interfaces.IProductService, WebStore.Services.ConcreteServices.ProductService>();
        services.AddTransient<WebStore.Services.Interfaces.IOrderService, WebStore.Services.ConcreteServices.OrderService>();
        services.AddTransient<WebStore.Services.Interfaces.IInvoiceService, WebStore.Services.ConcreteServices.InvoiceService>();
        services.AddTransient<WebStore.Services.Interfaces.IStoreService, WebStore.Services.ConcreteServices.StoreService>();
        services.AddTransient<WebStore.Services.Interfaces.IAddressService, WebStore.Services.ConcreteServices.AddressService>();

        ServiceProvider = services.BuildServiceProvider();
        DbContext = ServiceProvider.GetRequiredService<WebStoreDbContext>();
        UserManager = ServiceProvider.GetRequiredService<UserManager<User>>();
    }

    public async Task InitializeAsync()
    {
        await SeedTestDataAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    private async Task SeedTestDataAsync()
    {
        // Suppliers
        var supplier1 = new Supplier()
        {
            FirstName = "Adam",
            LastName = "Bednarski",
            UserName = "supp1@eg.eg",
            Email = "supp1@eg.eg",
            RegistrationDate = new DateTime(2010, 1, 1),
            CompanyName = "ABC Supplies"
        };
        await UserManager.CreateAsync(supplier1, "User1234");
        await DbContext.SaveChangesAsync();

        var createdSupplier = await DbContext.Suppliers.FirstOrDefaultAsync(s => s.UserName == "supp1@eg.eg");

        // Categories
        var category1 = new Category()
        {
            Name = "Computers",
            Description = "Computer category"
        };
        await DbContext.AddAsync(category1);
        await DbContext.SaveChangesAsync();

        var createdCategory = await DbContext.Categories.FirstOrDefaultAsync(c => c.Name == "Computers");

        // Products
        var p1 = new Product()
        {
            CategoryId = createdCategory!.Id,
            SupplierId = createdSupplier!.Id,
            Description = "Bardzo praktyczny monitor 32 cale",
            ImageBytes = new byte[] { 0xff, 0xff, 0xff, 0x80 },
            Name = "Monitor Dell 32",
            Price = 1000,
            Weight = 20,
        };
        await DbContext.AddAsync(p1);

        var p2 = new Product()
        {
            CategoryId = createdCategory.Id,
            SupplierId = createdSupplier.Id,
            Description = "Precyzyjna mysz do pracy",
            ImageBytes = new byte[] { 0xff, 0xff, 0xff, 0x70 },
            Name = "Mysz Logitech",
            Price = 500,
            Weight = 0.5f,
        };
        await DbContext.AddAsync(p2);
        await DbContext.SaveChangesAsync();

        // Customers
        var customer1 = new Customer()
        {
            FirstName = "Jan",
            LastName = "Kowalski",
            UserName = "customer1@eg.eg",
            Email = "customer1@eg.eg",
            RegistrationDate = new DateTime(2020, 1, 1),
            BillingAddress = new Address
            {
                Street = "ul. Billingowa 1",
                City = "Warszawa",
                PostalCode = "00-001",
                Country = "Poland"
            },
            ShippingAddress = new Address
            {
                Street = "ul. Shippingowa 2",
                City = "Warszawa",
                PostalCode = "00-002",
                Country = "Poland"
            }
        };
        await UserManager.CreateAsync(customer1, "User1234");
        await DbContext.SaveChangesAsync();

        var createdCustomer = await DbContext.Customers.FirstOrDefaultAsync(c => c.UserName == "customer1@eg.eg");

        // Orders
        var order1 = new Order()
        {
            OrderDate = DateTime.Now.AddDays(-10),
            DeliveryDate = DateTime.Now.AddDays(-3),
            OrderAmount = 1500,
            TrackingNumber = 123456789,
            CustomerId = createdCustomer!.Id,
            OrderProducts = new List<OrderProduct>
            {
                new OrderProduct { ProductId = p1.Id, Quantity = 1 },
                new OrderProduct { ProductId = p2.Id, Quantity = 1 }
            }
        };
        await DbContext.AddAsync(order1);
        await DbContext.SaveChangesAsync();

        var createdOrder = await DbContext.Orders.FirstOrDefaultAsync(o => o.TrackingNumber == 123456789);

        // Invoices
        var invoice1 = new Invoice()
        {
            InvoiceNumber = "INV-001",
            IssueDate = DateTime.Now.AddDays(-10),
            DueDate = DateTime.Now.AddDays(20),
            Amount = 1500,
            OrderId = createdOrder!.Id
        };
        await DbContext.AddAsync(invoice1);

        // Stores
        var store1 = new StationaryStore()
        {
            Name = "Store Warszawa",
            Address = new Address
            {
                Street = "ul. Store 10",
                City = "Warszawa",
                PostalCode = "00-100",
                Country = "Poland"
            },
            Employees = new List<StationaryStoreEmployee>()
        };
        await DbContext.AddAsync(store1);

        await DbContext.SaveChangesAsync();
    }

    public void Dispose()
    {
        DbContext?.Dispose();
    }
}


using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebStore.DAL.EF;
using WebStore.Model.DataModels;
namespace WebStore.Tests
{
    public static class Extensions
    {
        // Create sample data
        public static async void SeedData(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var dbContext = serviceProvider.GetRequiredService<WebStoreDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = serviceProvider
            .GetRequiredService<RoleManager<IdentityRole<int>>>();
            // other seed data ...
            //Suppliers
            var supplier1 = new Supplier()
            {
                Id = 1,
                FirstName = "Adam",
                LastName = "Bednarski",
                UserName = "supp1@eg.eg",
                Email = "supp1@eg.eg",
                RegistrationDate = new DateTime(2010, 1, 1),
                CompanyName = "ABC Supplies"
            };
            await userManager.CreateAsync(supplier1, "User1234");
            //Categories
            var category1 = new Category()
            {
                Id = 1,
                Name = "Computers",
                Description = "Computer category"
            };
            await dbContext.AddAsync(category1);
            //Products
            var p1 = new Product()
            {
                Id = 1,
                CategoryId = category1.Id,
                SupplierId = supplier1.Id,
                Description = "Bardzo praktyczny monitor 32 cale",
                ImageBytes = new byte[] { 0xff, 0xff, 0xff, 0x80 },
                Name = "Monitor Dell 32",
                Price = 1000,
                Weight = 20,
            };
            await dbContext.AddAsync(p1);
            var p2 = new Product()
            {
                Id = 2,
                CategoryId = category1.Id,
                SupplierId = supplier1.Id,
                Description = "Precyzyjna mysz do pracy",
                ImageBytes = new byte[] { 0xff, 0xff, 0xff, 0x70 },
                Name = "Mysz Logitech",
                Price = 500,
                Weight = 0.5f,
            };
            await dbContext.AddAsync(p2);
            
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
            await userManager.CreateAsync(customer1, "User1234");
            await dbContext.SaveChangesAsync();
            
            // Get the created customer from database
            var createdCustomer = await dbContext.Customers.FirstOrDefaultAsync(c => c.UserName == "customer1@eg.eg");
            
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
                    new OrderProduct { ProductId = 1, Quantity = 1 },
                    new OrderProduct { ProductId = 2, Quantity = 1 }
                }
            };
            await dbContext.AddAsync(order1);
            await dbContext.SaveChangesAsync();
            
            // Get the created order from database
            var createdOrder = await dbContext.Orders.FirstOrDefaultAsync(o => o.TrackingNumber == 123456789);
            
            // Invoices
            var invoice1 = new Invoice()
            {
                InvoiceNumber = "INV-001",
                IssueDate = DateTime.Now.AddDays(-10),
                DueDate = DateTime.Now.AddDays(20),
                Amount = 1500,
                OrderId = createdOrder!.Id
            };
            await dbContext.AddAsync(invoice1);
            
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
            await dbContext.AddAsync(store1);
            
            // save changes
            await dbContext.SaveChangesAsync();
        }
    }
}

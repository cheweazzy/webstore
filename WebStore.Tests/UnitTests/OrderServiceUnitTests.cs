using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using WebStore.DAL.EF;
using WebStore.Model.DataModels;
using WebStore.Services.Interfaces;
using WebStore.Tests.Fixtures;
using WebStore.ViewModels.VM;
using Xunit;

namespace WebStore.Tests.UnitTests;

public class OrderServiceUnitTests : IClassFixture<TestDataFixture>
{
    private readonly TestDataFixture _fixture;
    private readonly IOrderService _orderService;
    private readonly WebStoreDbContext _dbContext;

    public OrderServiceUnitTests(TestDataFixture fixture)
    {
        _fixture = fixture;
        _orderService = fixture.ServiceProvider.GetRequiredService<IOrderService>();
        _dbContext = fixture.DbContext;
    }

    [Fact]
    public void GetOrderTest()
    {
        // Get existing order from database
        var existingOrder = _dbContext.Orders.FirstOrDefault();
        if (existingOrder == null)
        {
            // No order exists, skip test
            return;
        }
        
        var order = _orderService.GetOrder(o => o.TrackingNumber == existingOrder.TrackingNumber);
        Assert.NotNull(order);
        Assert.Equal(existingOrder.TrackingNumber, order.TrackingNumber);
    }

    [Fact]
    public void GetMultipleOrdersTest()
    {
        var orders = _orderService.GetOrders(o => o.OrderAmount > 500);
        Assert.NotNull(orders);
        Assert.NotEmpty(orders);
    }

    [Fact]
    public void GetAllOrdersTest()
    {
        var orders = _orderService.GetOrders();
        Assert.NotNull(orders);
        // May be empty if no orders exist
    }

    [Fact]
    public void AddNewOrderTest()
    {
        // Get existing customer and products
        var customer = _dbContext.Customers.First();
        var products = _dbContext.Products.Take(2).ToList();
        
        var newOrderVm = new AddOrUpdateOrderVm()
        {
            OrderDate = DateTime.Now,
            DeliveryDate = DateTime.Now.AddDays(7),
            OrderAmount = 1500,
            TrackingNumber = 999888777,
            CustomerId = customer.Id,
            OrderProducts = new List<OrderProductItemVm>
            {
                new OrderProductItemVm { ProductId = products[0].Id, Quantity = 2 },
                new OrderProductItemVm { ProductId = products[1].Id, Quantity = 1 }
            }
        };

        var createdOrder = _orderService.AddOrUpdateOrder(newOrderVm);
        Assert.NotNull(createdOrder);
        Assert.Equal(999888777, createdOrder.TrackingNumber);
        Assert.Equal(2, createdOrder.OrderProducts.Count);
    }

    [Fact]
    public void UpdateOrderTest()
    {
        // Get existing order
        var existingOrder = _dbContext.Orders.First();
        var products = _dbContext.Products.Take(1).ToList();
        
        var updateOrderVm = new AddOrUpdateOrderVm()
        {
            Id = existingOrder.Id,
            OrderDate = DateTime.Now,
            DeliveryDate = DateTime.Now.AddDays(10),
            OrderAmount = 2000,
            TrackingNumber = existingOrder.TrackingNumber,
            CustomerId = existingOrder.CustomerId,
            OrderProducts = new List<OrderProductItemVm>
            {
                new OrderProductItemVm { ProductId = products[0].Id, Quantity = 3 }
            }
        };

        var editedOrder = _orderService.AddOrUpdateOrder(updateOrderVm);
        Assert.NotNull(editedOrder);
        Assert.Equal(existingOrder.TrackingNumber, editedOrder.TrackingNumber);
        Assert.Equal(2000, editedOrder.OrderAmount);
    }

    [Fact]
    public void DeleteOrderTest()
    {
        // Get existing order
        var existingOrder = _dbContext.Orders.First();
        var result = _orderService.DeleteOrder(existingOrder.Id);
        Assert.True(result);
    }
}


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

public class OrderService : BaseService, IOrderService
{
    public OrderService(WebStoreDbContext dbContext, AutoMapper.IMapper mapper, ILogger<OrderService> logger)
        : base(dbContext, mapper, logger)
    {
    }

    public OrderVm AddOrUpdateOrder(AddOrUpdateOrderVm addOrUpdateOrderVm)
    {
        Order order;

        if (addOrUpdateOrderVm.Id.HasValue && addOrUpdateOrderVm.Id.Value > 0)
        {
            // Update existing order
            order = DbContext.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Product)
                .FirstOrDefault(o => o.Id == addOrUpdateOrderVm.Id.Value)
                ?? throw new ArgumentException($"Order with id {addOrUpdateOrderVm.Id.Value} not found.");

            order.OrderDate = addOrUpdateOrderVm.OrderDate;
            order.DeliveryDate = addOrUpdateOrderVm.DeliveryDate;
            order.OrderAmount = addOrUpdateOrderVm.OrderAmount;
            order.TrackingNumber = addOrUpdateOrderVm.TrackingNumber;
            order.CustomerId = addOrUpdateOrderVm.CustomerId;

            // Update OrderProducts
            DbContext.OrderProducts.RemoveRange(order.OrderProducts);
            foreach (var item in addOrUpdateOrderVm.OrderProducts)
            {
                order.OrderProducts.Add(new OrderProduct
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                });
            }
        }
        else
        {
            // Create new order
            order = new Order
            {
                OrderDate = addOrUpdateOrderVm.OrderDate,
                DeliveryDate = addOrUpdateOrderVm.DeliveryDate,
                OrderAmount = addOrUpdateOrderVm.OrderAmount,
                TrackingNumber = addOrUpdateOrderVm.TrackingNumber,
                CustomerId = addOrUpdateOrderVm.CustomerId,
                OrderProducts = new List<OrderProduct>()
            };

            foreach (var item in addOrUpdateOrderVm.OrderProducts)
            {
                order.OrderProducts.Add(new OrderProduct
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                });
            }

            DbContext.Orders.Add(order);
        }

        DbContext.SaveChanges();

        return MapToOrderVm(order);
    }

    public OrderVm GetOrder(Expression<Func<Order, bool>> filterExpression)
    {
        var order = DbContext.Orders
            .Include(o => o.Customer)
            .Include(o => o.Invoice)
            .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
            .FirstOrDefault(filterExpression);

        if (order == null)
        {
            throw new ArgumentException("Order not found.");
        }

        return MapToOrderVm(order);
    }

    public IEnumerable<OrderVm> GetOrders(Expression<Func<Order, bool>>? filterExpression = null)
    {
        IQueryable<Order> query = DbContext.Orders
            .Include(o => o.Customer)
            .Include(o => o.Invoice)
            .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product);

        if (filterExpression != null)
        {
            query = query.Where(filterExpression);
        }

        var orders = query.ToList();

        return orders.Select(MapToOrderVm);
    }

    public bool DeleteOrder(int orderId)
    {
        var order = DbContext.Orders.FirstOrDefault(o => o.Id == orderId);
        if (order == null)
        {
            return false;
        }

        DbContext.Orders.Remove(order);
        DbContext.SaveChanges();
        return true;
    }

    private OrderVm MapToOrderVm(Order order)
    {
        return new OrderVm
        {
            Id = order.Id,
            OrderDate = order.OrderDate,
            DeliveryDate = order.DeliveryDate,
            OrderAmount = order.OrderAmount,
            TrackingNumber = order.TrackingNumber,
            CustomerId = order.CustomerId,
            CustomerName = order.Customer != null ? $"{order.Customer.FirstName} {order.Customer.LastName}" : null,
            InvoiceId = order.Invoice?.Id,
            OrderProducts = order.OrderProducts?.Select(op => new OrderProductVm
            {
                ProductId = op.ProductId,
                ProductName = op.Product?.Name ?? "Unknown",
                Quantity = op.Quantity,
                UnitPrice = op.Product?.Price ?? 0
            }).ToList() ?? new List<OrderProductVm>()
        };
    }
}


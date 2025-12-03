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

public class InvoiceService : BaseService, IInvoiceService
{
    public InvoiceService(WebStoreDbContext dbContext, AutoMapper.IMapper mapper, ILogger<InvoiceService> logger)
        : base(dbContext, mapper, logger)
    {
    }

    public InvoiceVm AddOrUpdateInvoice(AddOrUpdateInvoiceVm addOrUpdateInvoiceVm)
    {
        Invoice invoice;

        if (addOrUpdateInvoiceVm.Id.HasValue && addOrUpdateInvoiceVm.Id.Value > 0)
        {
            // Update existing invoice
            invoice = DbContext.Invoices
                .Include(i => i.Order)
                .FirstOrDefault(i => i.Id == addOrUpdateInvoiceVm.Id.Value)
                ?? throw new ArgumentException($"Invoice with id {addOrUpdateInvoiceVm.Id.Value} not found.");

            invoice.InvoiceNumber = addOrUpdateInvoiceVm.InvoiceNumber;
            invoice.IssueDate = addOrUpdateInvoiceVm.IssueDate;
            invoice.DueDate = addOrUpdateInvoiceVm.DueDate;
            invoice.Amount = addOrUpdateInvoiceVm.Amount;
            invoice.OrderId = addOrUpdateInvoiceVm.OrderId;
        }
        else
        {
            // Check if order exists
            var order = DbContext.Orders.FirstOrDefault(o => o.Id == addOrUpdateInvoiceVm.OrderId);
            if (order == null)
            {
                throw new ArgumentException($"Order with id {addOrUpdateInvoiceVm.OrderId} not found.");
            }

            // Check if invoice already exists for this order
            var existingInvoice = DbContext.Invoices.FirstOrDefault(i => i.OrderId == addOrUpdateInvoiceVm.OrderId);
            if (existingInvoice != null)
            {
                throw new InvalidOperationException($"Invoice already exists for order {addOrUpdateInvoiceVm.OrderId}.");
            }

            // Create new invoice
            invoice = new Invoice
            {
                InvoiceNumber = addOrUpdateInvoiceVm.InvoiceNumber,
                IssueDate = addOrUpdateInvoiceVm.IssueDate,
                DueDate = addOrUpdateInvoiceVm.DueDate,
                Amount = addOrUpdateInvoiceVm.Amount,
                OrderId = addOrUpdateInvoiceVm.OrderId
            };

            DbContext.Invoices.Add(invoice);
        }

        DbContext.SaveChanges();

        return MapToInvoiceVm(invoice);
    }

    public InvoiceVm GetInvoice(Expression<Func<Invoice, bool>> filterExpression)
    {
        var invoice = DbContext.Invoices
            .Include(i => i.Order)
            .FirstOrDefault(filterExpression);

        if (invoice == null)
        {
            throw new ArgumentException("Invoice not found.");
        }

        return MapToInvoiceVm(invoice);
    }

    public IEnumerable<InvoiceVm> GetInvoices(Expression<Func<Invoice, bool>>? filterExpression = null)
    {
        IQueryable<Invoice> query = DbContext.Invoices
            .Include(i => i.Order);

        if (filterExpression != null)
        {
            query = query.Where(filterExpression);
        }

        var invoices = query.ToList();

        return invoices.Select(MapToInvoiceVm);
    }

    public bool DeleteInvoice(int invoiceId)
    {
        var invoice = DbContext.Invoices.FirstOrDefault(i => i.Id == invoiceId);
        if (invoice == null)
        {
            return false;
        }

        DbContext.Invoices.Remove(invoice);
        DbContext.SaveChanges();
        return true;
    }

    private InvoiceVm MapToInvoiceVm(Invoice invoice)
    {
        return new InvoiceVm
        {
            Id = invoice.Id,
            InvoiceNumber = invoice.InvoiceNumber,
            IssueDate = invoice.IssueDate,
            DueDate = invoice.DueDate,
            Amount = invoice.Amount,
            OrderId = invoice.OrderId,
            OrderTrackingNumber = invoice.Order?.TrackingNumber
        };
    }
}


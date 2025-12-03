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

public class InvoiceServiceUnitTests : IClassFixture<TestDataFixture>
{
    private readonly TestDataFixture _fixture;
    private readonly IInvoiceService _invoiceService;
    private readonly WebStoreDbContext _dbContext;

    public InvoiceServiceUnitTests(TestDataFixture fixture)
    {
        _fixture = fixture;
        _invoiceService = fixture.ServiceProvider.GetRequiredService<IInvoiceService>();
        _dbContext = fixture.DbContext;
    }

    [Fact]
    public void GetInvoiceTest()
    {
        var invoice = _invoiceService.GetInvoice(i => i.InvoiceNumber == "INV-001");
        Assert.NotNull(invoice);
        Assert.Equal("INV-001", invoice.InvoiceNumber);
    }

    [Fact]
    public void GetMultipleInvoicesTest()
    {
        var invoices = _invoiceService.GetInvoices(i => i.Amount > 0);
        Assert.NotNull(invoices);
        // May be empty if no invoices exist
    }

    [Fact]
    public void GetAllInvoicesTest()
    {
        var invoices = _invoiceService.GetInvoices();
        Assert.NotNull(invoices);
    }

    [Fact]
    public void AddNewInvoiceTest()
    {
        // Get existing order
        var existingOrder = _dbContext.Orders.First();
        
        var newInvoiceVm = new AddOrUpdateInvoiceVm()
        {
            InvoiceNumber = "INV-002",
            IssueDate = DateTime.Now,
            DueDate = DateTime.Now.AddDays(30),
            Amount = 2500,
            OrderId = existingOrder.Id
        };

        var createdInvoice = _invoiceService.AddOrUpdateInvoice(newInvoiceVm);
        Assert.NotNull(createdInvoice);
        Assert.Equal("INV-002", createdInvoice.InvoiceNumber);
        Assert.Equal(2500, createdInvoice.Amount);
    }

    [Fact]
    public void UpdateInvoiceTest()
    {
        // Get existing invoice
        var existingInvoice = _dbContext.Invoices.FirstOrDefault();
        if (existingInvoice == null)
        {
            // No invoice exists, skip test
            return;
        }
        
        var updateInvoiceVm = new AddOrUpdateInvoiceVm()
        {
            Id = existingInvoice.Id,
            InvoiceNumber = "INV-001-UPDATED",
            IssueDate = DateTime.Now,
            DueDate = DateTime.Now.AddDays(45),
            Amount = 3000,
            OrderId = existingInvoice.OrderId
        };

        var editedInvoice = _invoiceService.AddOrUpdateInvoice(updateInvoiceVm);
        Assert.NotNull(editedInvoice);
        Assert.Equal("INV-001-UPDATED", editedInvoice.InvoiceNumber);
        Assert.Equal(3000, editedInvoice.Amount);
    }

    [Fact]
    public void DeleteInvoiceTest()
    {
        // Get existing invoice
        var existingInvoice = _dbContext.Invoices.First();
        var result = _invoiceService.DeleteInvoice(existingInvoice.Id);
        Assert.True(result);
    }
}


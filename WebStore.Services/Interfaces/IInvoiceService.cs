using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using WebStore.Model.DataModels;
using WebStore.ViewModels.VM;

namespace WebStore.Services.Interfaces;

public interface IInvoiceService
{
    InvoiceVm AddOrUpdateInvoice(AddOrUpdateInvoiceVm addOrUpdateInvoiceVm);
    InvoiceVm GetInvoice(Expression<Func<Invoice, bool>> filterExpression);
    IEnumerable<InvoiceVm> GetInvoices(Expression<Func<Invoice, bool>>? filterExpression = null);
    bool DeleteInvoice(int invoiceId);
}


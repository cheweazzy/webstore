using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using WebStore.Model.DataModels;
using WebStore.ViewModels.VM;

namespace WebStore.Services.Interfaces
{
    public interface IProductService
    {
        ProductVm AddOrUpdateProduct(AddOrUpdateProductVm addOrUpdateProductVm);
        ProductVm GetProduct(Expression<Func<Product, bool>> filterExpression);
        IEnumerable<ProductVm> GetProducts(Expression<Func<Product, bool>>? filterExpression = null);
        bool DeleteProduct(Expression<Func<Product, bool>> filterExpression);
    }
}
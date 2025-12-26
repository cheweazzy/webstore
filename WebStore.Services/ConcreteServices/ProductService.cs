using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using Microsoft.Extensions.Logging;
using WebStore.DAL.EF;
using WebStore.Model.DataModels;
using WebStore.Services.Interfaces;
using WebStore.ViewModels.VM;
namespace WebStore.Services.ConcreteServices
{
    public class ProductService : BaseService, IProductService
    {
        public ProductService(WebStoreDbContext dbContext, IMapper mapper, ILogger<ProductService> logger)
        : base(dbContext, mapper, logger) { }
        public ProductVm AddOrUpdateProduct(AddOrUpdateProductVm addOrUpdateProductVm)
        {
            try
            {
                if (addOrUpdateProductVm == null)
                    throw new ArgumentNullException("View model parameter is null");
                Product productEntity;
                if (addOrUpdateProductVm.Id.HasValue && addOrUpdateProductVm.Id.Value > 0)
                {
                    productEntity = DbContext.Products.FirstOrDefault(p => p.Id == addOrUpdateProductVm.Id.Value)
                        ?? throw new ArgumentException($"Product with id {addOrUpdateProductVm.Id.Value} not found.");
                    Mapper.Map(addOrUpdateProductVm, productEntity);
                    DbContext.Products.Update(productEntity);
                }
                else
                {
                    productEntity = Mapper.Map<Product>(addOrUpdateProductVm);
                    DbContext.Products.Add(productEntity);
                }
                DbContext.SaveChanges();
                var productVm = Mapper.Map<ProductVm>(productEntity);
                // Get quantity from ProductStock
                var productStock = DbContext.ProductStocks.FirstOrDefault(ps => ps.ProductId == productEntity.Id);
                productVm.Quantity = productStock?.Quantity ?? 0;
                return productVm;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw;
            }
        }
        public ProductVm GetProduct(Expression<Func<Product, bool>> filterExpression)
        {
            try
            {
                if (filterExpression == null)
                    throw new ArgumentNullException("Filter expression parameter is null");
                var productEntity = DbContext.Products.FirstOrDefault(filterExpression);
                if (productEntity == null)
                    throw new ArgumentException("Product not found.");
                var productVm = Mapper.Map<ProductVm>(productEntity);
                // Get quantity from ProductStock
                var productStock = DbContext.ProductStocks.FirstOrDefault(ps => ps.ProductId == productEntity.Id);
                productVm.Quantity = productStock?.Quantity ?? 0;
                return productVm;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw;
            }
        }
        public IEnumerable<ProductVm> GetProducts(Expression<Func<Product, bool>>? filterExpression = null)
        {
            try
            {
                var productsQuery = DbContext.Products.AsQueryable();
                if (filterExpression != null)
                    productsQuery = productsQuery.Where(filterExpression);
                var products = productsQuery.ToList();
                var productVms = products.Select(product =>
                {
                    var productVm = Mapper.Map<ProductVm>(product);
                    // Get quantity from ProductStock
                    var productStock = DbContext.ProductStocks.FirstOrDefault(ps => ps.ProductId == product.Id);
                    productVm.Quantity = productStock?.Quantity ?? 0;
                    return productVm;
                });
                return productVms;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw;
            }
        }
        // ... (reszta kodu bez zmian)

        // --- WKLEJ TO NA KOŃCU KLASY ProductService ---
        public bool DeleteProduct(Expression<Func<Product, bool>> filterExpression)
        {
            try
            {
                if (filterExpression == null)
                    throw new ArgumentNullException("Filter expression is null");

                var productEntity = DbContext.Products.FirstOrDefault(filterExpression);
                
                if (productEntity == null)
                    return false; // Produkt nie istnieje, więc nie usuwamy

                DbContext.Products.Remove(productEntity);
                DbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw;
            }
        }
    } // Koniec klasy
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using WebStore.DAL.EF;
using WebStore.Model.DataModels;
using WebStore.Services.Interfaces;
using WebStore.Tests.Fixtures;
using WebStore.ViewModels.VM;
using Xunit;

namespace WebStore.Tests.UnitTests;

public class ProductServiceUnitTests : IClassFixture<TestDataFixture>
{
    private readonly TestDataFixture _fixture;
    private readonly IProductService _productService;
    private readonly WebStoreDbContext _dbContext;

    public ProductServiceUnitTests(TestDataFixture fixture)
    {
        _fixture = fixture;
        _productService = fixture.ServiceProvider.GetRequiredService<IProductService>();
        _dbContext = fixture.DbContext;
    }
        [Fact]
        public void GetProductTest()
        {
            var product = _productService.GetProduct(p => p.Name == "Monitor Dell 32");
            Assert.NotNull(product);
        }
        [Fact]
        public void GetMultipleProductsTest()
        {
            var allProducts = _dbContext.Products.ToList();
            if (allProducts.Count < 2)
            {
                // Not enough products, skip test
                return;
            }
            var firstProductId = allProducts[0].Id;
            var secondProductId = allProducts[1].Id;
            var products = _productService.GetProducts(p => p.Id >= firstProductId && p.Id <= secondProductId);
            Assert.NotNull(products);
            Assert.NotEmpty(products);
            Assert.True(products.Count() >= 1);
        }
        [Fact]
        public void GetAllProductsTest()
        {
            var products = _productService.GetProducts();
            Assert.NotNull(products);
            Assert.NotEmpty(products);
            Assert.True(products.Count() > 0);
        }
        [Fact]
        public void AddNewProductTest()
        {
            // Get existing category and supplier
            var category = _dbContext.Categories.First();
            var supplier = _dbContext.Suppliers.First();
            
            var newProductVm = new AddOrUpdateProductVm()
            {
                Name = "MacBook Pro",
                CategoryId = category.Id,
                SupplierId = supplier.Id,
                ImageBytes = new byte[] {
0xff,
0xff,
0xff,
0x80
},
                Price = 6000,
                Weight = 1.1f,
                Description = "MacBook Pro z procesorem M1 8GB RAM, Dysk 256 GB"
            };
            var createdProduct = _productService.AddOrUpdateProduct(newProductVm);
            Assert.NotNull(createdProduct);
            Assert.Equal("MacBook Pro", createdProduct.Name);
        }
        [Fact]
        public void UpdateProductTest()
        {
            // Get existing product
            var existingProduct = _dbContext.Products.First();
            
            var updateProductVm = new AddOrUpdateProductVm()
            {
                Id = existingProduct.Id,
                Description = "Bardzo praktyczny monitor 32 cale - UPDATED",
                ImageBytes = new byte[] { 0xff, 0xff, 0xff, 0x80 },
                Name = "Monitor Dell 32",
                Price = 2000,
                Weight = 20,
                CategoryId = existingProduct.CategoryId,
                SupplierId = existingProduct.SupplierId
            };
            var editedProductVm = _productService.AddOrUpdateProduct(updateProductVm);
            Assert.NotNull(editedProductVm);
            Assert.Equal("Monitor Dell 32", editedProductVm.Name);
            Assert.Equal(2000, editedProductVm.Price);
        }
}
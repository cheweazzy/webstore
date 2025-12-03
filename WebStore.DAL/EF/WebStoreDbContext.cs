using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebStore.Model.DataModels;

namespace WebStore.DAL.EF;

public class WebStoreDbContext : IdentityDbContext<User, IdentityRole<int>, int> {
    public WebStoreDbContext(DbContextOptions<WebStoreDbContext> options) : base(options) {
    }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<StationaryStoreEmployee> StationaryStoreEmployees { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<OrderProduct> OrderProducts { get; set; }
    public DbSet<ProductStock> ProductStocks { get; set; }
    public DbSet<StationaryStore> StationaryStores { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);

        // TPH (Table Per Hierarchy) dla dziedziczenia User -> Customer, Supplier, StationaryStoreEmployee
        modelBuilder.Entity<User>()
            .HasDiscriminator<string>("UserType")
            .HasValue<User>("User")
            .HasValue<Customer>("Customer")
            .HasValue<Supplier>("Supplier")
            .HasValue<StationaryStoreEmployee>("StationaryStoreEmployee");

        // Relacje 1:M

        // Customer -> Order (1:M)
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Customer)
            .WithMany(c => c.Orders)
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Supplier -> Product (1:M)
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Supplier)
            .WithMany(s => s.Products)
            .HasForeignKey(p => p.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        // Product -> ProductStock (1:M)
        modelBuilder.Entity<ProductStock>()
            .HasOne(ps => ps.Product)
            .WithMany()
            .HasForeignKey(ps => ps.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // Category -> Product (1:M)
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Invoice -> Order (1:1, ale Invoice jest opcjonalny)
        modelBuilder.Entity<Invoice>()
            .HasOne(i => i.Order)
            .WithOne(o => o.Invoice)
            .HasForeignKey<Invoice>(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Customer -> Address (1:M dla BillingAddress i ShippingAddress)
        // Address jest własnością Customer (Owned Entity)
        modelBuilder.Entity<Customer>()
            .OwnsOne(c => c.BillingAddress, ba => {
                ba.Property(a => a.Street).HasColumnName("BillingStreet");
                ba.Property(a => a.City).HasColumnName("BillingCity");
                ba.Property(a => a.PostalCode).HasColumnName("BillingPostalCode");
                ba.Property(a => a.Country).HasColumnName("BillingCountry");
            });

        modelBuilder.Entity<Customer>()
            .OwnsOne(c => c.ShippingAddress, sa => {
                sa.Property(a => a.Street).HasColumnName("ShippingStreet");
                sa.Property(a => a.City).HasColumnName("ShippingCity");
                sa.Property(a => a.PostalCode).HasColumnName("ShippingPostalCode");
                sa.Property(a => a.Country).HasColumnName("ShippingCountry");
            });

        // StationaryStore -> Address (1:1, Address jako owned entity)
        modelBuilder.Entity<StationaryStore>()
            .OwnsOne(s => s.Address, a => {
                a.Property(addr => addr.Street).HasColumnName("Street");
                a.Property(addr => addr.City).HasColumnName("City");
                a.Property(addr => addr.PostalCode).HasColumnName("PostalCode");
                a.Property(addr => addr.Country).HasColumnName("Country");
            });

        // StationaryStore -> StationaryStoreEmployee (1:M)
        modelBuilder.Entity<StationaryStoreEmployee>()
            .HasOne(e => e.StationaryStore)
            .WithMany(s => s.Employees)
            .HasForeignKey(e => e.StationaryStoreId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relacja N:M Order -> Product przez OrderProduct
        modelBuilder.Entity<OrderProduct>()
            .HasKey(op => op.Id);

        modelBuilder.Entity<OrderProduct>()
            .HasOne(op => op.Order)
            .WithMany(o => o.OrderProducts)
            .HasForeignKey(op => op.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<OrderProduct>()
            .HasOne(op => op.Product)
            .WithMany(p => p.OrderProducts)
            .HasForeignKey(op => op.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // Unikalny indeks dla pary Order-Product (opcjonalnie, jeśli nie chcemy duplikatów)
        modelBuilder.Entity<OrderProduct>()
            .HasIndex(op => new { op.OrderId, op.ProductId })
            .IsUnique();
    }
}


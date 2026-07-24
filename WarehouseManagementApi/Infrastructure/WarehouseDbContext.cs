using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class WarehouseDbContext(DbContextOptions<WarehouseDbContext> options)
    : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<WarehouseItem> WarehouseItems => Set<WarehouseItem>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();
    public DbSet<SupplierDocument> SupplierDocuments => Set<SupplierDocument>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.Entity<Product>()
            .HasOne(p => p.Supplier)
            .WithMany()
            .HasForeignKey(p => p.SupplierId);
        
        builder.Entity<ProductImage>()
            .HasOne(i => i.Product)
            .WithMany()
            .HasForeignKey(i => i.ProductId);
        
        builder.Entity<WarehouseItem>()
            .HasOne(i => i.Product)
            .WithMany()
            .HasForeignKey(i => i.ProductId);
        
        builder.Entity<StockMovement>()
            .HasOne(m => m.WarehouseItem)
            .WithMany()
            .HasForeignKey(m => m.WarehouseItemId);
        
        builder.Entity<SupplierDocument>()
            .HasOne(i => i.Supplier)
            .WithMany()
            .HasForeignKey(i => i.SupplierId);
    }
}
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SenjaCoffee.Data.Models;

namespace SenjaCoffee.Data
{
    public class SenjaDbContext : IdentityDbContext
    {
       public SenjaDbContext() {}

       public SenjaDbContext(DbContextOptions options) : base(options) {}
       
       public virtual DbSet<Customer> Customers { get; set; }
       public virtual DbSet<CustomerAddress> CustomerAddress { get; set; }
       public virtual DbSet<Product> Products { get; set; }
       public virtual DbSet<ProductInventory> ProductInventories { get; set; }
       public virtual DbSet<ProductInventorySnapshot> ProductInventorySnapshots { get; set; }
       public virtual DbSet<SalesOrder> SalesOrders { get; set; }
       public virtual DbSet<SalesOrderItem> SalesOrderItems { get; set; }
    }
}
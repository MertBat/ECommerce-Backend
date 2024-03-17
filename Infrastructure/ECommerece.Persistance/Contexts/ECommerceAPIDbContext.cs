using ECommerce.Domain.Entities;
using ECommerce.Domain.Entities.Common;
using ECommerce.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Persistance.Contexts
{
    public class ECommerceAPIDbContext : IdentityDbContext<AppUser, AppRole, string>
    {

        public ECommerceAPIDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Domain.Entities.File> Files { get; set; }
        public DbSet<ProductImageFile> ProductImageFiles { get; set; }
        public DbSet<InvoiceFile> InvoiceFİles { get; set; }
        public DbSet<Basket> Baskets { get; set; }
        public DbSet<BasketItem> BasketItems { get; set; }
        public DbSet<CompletedOrder> CompletedOrders { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Order>().HasKey(b => b.Id);

            builder.Entity<Order>().HasIndex(o=> o.OrderCode).IsUnique();

            builder.Entity<Basket>().HasOne(b => b.Order)
                .WithOne(o => o.Basket)
                .HasForeignKey<Order>(b => b.Id);

            builder.Entity<Order>().HasOne(o=> o.CompletedOrder)
                .WithOne(co=> co.Order)
                .HasForeignKey<CompletedOrder>(co => co.OrderId);

            base.OnModelCreating(builder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // ChangeTracker enititler üzerinde yapılan değişiklikleri ya da yeni eklenen verilerin yakalanmasını sağlayan property'dir. Update operasyonlarında Track edilen verileri yakalayıp elde etmemizi sağlar.

            var datas = ChangeTracker.Entries<BaseEntity>();
            foreach (var data in datas)
            {
                _ = data.State switch
                {
                    EntityState.Added => data.Entity.CreatedDate = DateTime.UtcNow,
                    EntityState.Modified => data.Entity.UpdatedDate = DateTime.UtcNow,
                    _ => DateTime.UtcNow
                };
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}

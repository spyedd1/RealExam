using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GFLHApp.Models;
using System.Linq;
using System.Reflection.Emit;

namespace GFLHApp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Basket> Basket { get; set; } = default!;
        public DbSet<BasketProducts> BasketProducts { get; set; } = default!;
        public DbSet<OrderProducts> OrderProducts { get; set; } = default!;
        public DbSet<Orders> Orders { get; set; } = default!;
        public DbSet<Producers> Producers { get; set; } = default!;
        public DbSet<Products> Products { get; set; } = default!;

        // This method is used to configurre the model and set the column type for all decimal properties to "decimal(10,2)" to ensure that they are stored in the database with the correct precision and scale
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            foreach (var property in builder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetColumnType("decimal(10,2)");
            }

            // Ensure UserId is unique so it can be used as a principal key
            builder.Entity<Producers>()
                .HasIndex(p => p.UserId)
                .IsUnique();

            builder.Entity<ProducerOrders>()
                .HasOne(po => po.Producers)
                .WithMany()
                .HasForeignKey(po => po.ProducerId)
                .HasPrincipalKey(p => p.UserId);
        }
        public DbSet<GFLHApp.Models.ProducerOrders> ProducerOrders { get; set; } = default!;
    }
}
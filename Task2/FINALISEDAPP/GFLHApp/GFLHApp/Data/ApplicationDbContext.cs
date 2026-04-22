// ----- Imports -----
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // Imports Microsoft.AspNetCore.Identity.EntityFrameworkCore types used by this file.
using Microsoft.EntityFrameworkCore; // Imports Microsoft.EntityFrameworkCore types used by this file.
using GFLHApp.Models; // Imports GFLHApp.Models types used by this file.
using System.Linq; // Imports System.Linq types used by this file.
using System.Reflection.Emit; // Imports System.Reflection.Emit types used by this file.

// ----- Namespace -----
namespace GFLHApp.Data // Places this class in the application data namespace.
{
    // ----- Database configuration -----
    public class ApplicationDbContext : IdentityDbContext // Defines the EF Core database context for the application.
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) // Receives DbContext options from dependency injection.
            : base(options) // Passes the configured options into the base Identity DbContext.
        {
        }

        // ----- DbSet properties -----
        public DbSet<Basket> Basket { get; set; } = default!; // Exposes the Basket table to EF Core queries.
        public DbSet<BasketProducts> BasketProducts { get; set; } = default!; // Exposes the BasketProducts table to EF Core queries.
        public DbSet<OrderProducts> OrderProducts { get; set; } = default!; // Exposes the OrderProducts table to EF Core queries.
        public DbSet<Orders> Orders { get; set; } = default!; // Exposes the Orders table to EF Core queries.
        public DbSet<Producers> Producers { get; set; } = default!; // Exposes the Producers table to EF Core queries.
        public DbSet<Products> Products { get; set; } = default!; // Exposes the Products table to EF Core queries.

        // ----- Database configuration -----
        protected override void OnModelCreating(ModelBuilder builder) // Configures EF Core model rules and relationships.
        {
            base.OnModelCreating(builder); // Configures EF Core model rules and relationships.

            foreach (var property in builder.Model.GetEntityTypes() // Loops through each item that needs seeding or configuration.
                .SelectMany(t => t.GetProperties()) // Flattens all entity properties into one sequence.
                // ----- Decimal precision -----
                .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?))) // Filters the sequence to the matching records or properties.
            {
                property.SetColumnType("decimal(10,2)"); // Stores decimal values using decimal(10,2) precision.
            }

            // ----- Producer relationship -----
            builder.Entity<Producers>() // Starts configuration for the Producers entity.
                .HasIndex(p => p.UserId) // Adds an index for producer user ids.
                .IsUnique(); // Requires each producer user id to be unique.

            builder.Entity<ProducerOrders>() // Starts configuration for the ProducerOrders entity.
                .HasOne(po => po.Producers) // Configures the producer order to reference one producer.
                .WithMany() // Allows the producer side to have many related orders.
                .HasForeignKey(po => po.ProducerId) // Uses ProducerId as the foreign key on producer orders.
                .HasPrincipalKey(p => p.UserId); // Uses producer UserId as the referenced principal key.
        }
        // ----- DbSet properties -----
        public DbSet<GFLHApp.Models.ProducerOrders> ProducerOrders { get; set; } = default!; // Exposes the ProducerOrders table to EF Core queries.
    }
}

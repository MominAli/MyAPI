using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext context, ILogger logger)
        {
            try
            {
                await context.Database.MigrateAsync();

                // Seed Users
                if (!context.Users.Any())
                {
                    var users = new List<User>
                    {
                        new User { Username = "admin", Email = "admin@example.com", PasswordHash = "admin123", Role = "Admin" },
                        new User { Username = "manager", Email = "manager@example.com", PasswordHash = "manager123", Role = "Manager" },
                        new User { Username = "user", Email = "user@example.com", PasswordHash = "user123", Role = "User" }
                    };
                    context.Users.AddRange(users);
                    await context.SaveChangesAsync();
                    logger.LogInformation("Seeded Users.");
                }

                // Seed Products
                if (!context.Products.Any())
                {
                    var products = new List<Product>
                    {
                        new Product { Sku = "SKU001", Name = "Laptop", Price = 999.99m, Stock = 10 },
                        new Product { Sku = "SKU002", Name = "Smartphone", Price = 499.50m, Stock = 25 },
                        new Product { Sku = "SKU003", Name = "Headphones", Price = 149.99m, Stock = 50 }
                    };
                    context.Products.AddRange(products);
                    await context.SaveChangesAsync();
                    logger.LogInformation("Seeded Products.");
                }

                // Seed Orders and OrderItems
                if (!context.Orders.Any())
                {
                    var user = await context.Users.FirstAsync(u => u.Username == "user");
                    var product1 = await context.Products.FirstAsync(p => p.Sku == "SKU001");
                    var product2 = await context.Products.FirstAsync(p => p.Sku == "SKU003");

                    var order = new Order
                    {
                        UserId = user.Id,
                        OrderedAt = DateTime.UtcNow,
                        Items = new List<OrderItem>
                        {
                            new OrderItem { ProductId = product1.Id, Quantity = 1, UnitPrice = product1.Price },
                            new OrderItem { ProductId = product2.Id, Quantity = 2, UnitPrice = product2.Price }
                        }
                    };

                    context.Orders.Add(order);
                    await context.SaveChangesAsync();
                    logger.LogInformation("Seeded Orders and OrderItems.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database.");
            }
        }
    }
}
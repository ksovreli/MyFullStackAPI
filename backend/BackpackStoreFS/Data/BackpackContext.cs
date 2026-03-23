using BackpackStoreFS.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BackpackStoreFS.Data
{
    public class BackpackContext(DbContextOptions<BackpackContext> options)
        : IdentityDbContext<User, IdentityRole<int>, int>(options)
    {
        public DbSet<Backpack> Backpacks { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<BasketItem> BasketsItems { get; set; }
        public DbSet<WishlistItem> WishlistItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
             
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Backpacks" },
                new Category { Id = 2, Name = "Duffel Bags" },
                new Category { Id = 3, Name = "Travel Packs" }
            );

            modelBuilder.Entity<Backpack>().HasData(
                new Backpack { Id = 1, Name = "APEX COMMUTER", Image = "/images/APEX_Commuter.png", Price = 90, Quantity = 10, SalePrice = 65, Rating = 4.5m, CategoryId = 1, IsNew = false },
                new Backpack { Id = 2, Name = "APEX HERITAGE", Image = "/images/APEX_Heritage.png", Price = 75, Quantity = 12, Rating = 4.1m, CategoryId = 1, IsNew = true },
                new Backpack { Id = 3, Name = "APEX PULSE", Image = "/images/APEX_Pulse.png", Price = 80, Quantity = 8, Rating = 4.8m, CategoryId = 1, IsNew = true },
                new Backpack { Id = 4, Name = "APEX STEALTH", Image = "/images/APEX_Stealth.png", Price = 110, Quantity = 6, SalePrice = 95, Rating = 4.2m, CategoryId = 1, IsNew = false },
                new Backpack { Id = 5, Name = "APEX SKYLINE", Image = "/images/APEX_Skyline.png", Price = 95, Quantity = 9, Rating = 4.8m, CategoryId = 1, IsNew = false },
                new Backpack { Id = 6, Name = "APEX GLOBAL", Image = "/images/APEX_Global.png", Price = 120, Quantity = 5, SalePrice = 85, Rating = 4.5m, CategoryId = 1, IsNew = false },

                new Backpack { Id = 7, Name = "APEX CROSSOVER", Image = "/images/APEX_Crossover.png", Price = 95, Quantity = 7, Rating = 4.0m, CategoryId = 2, IsNew = false },
                new Backpack { Id = 8, Name = "APEX EXECUTIVE", Image = "/images/APEX_Executive.png", Price = 150, Quantity = 4, SalePrice = 115, Rating = 4.9m, CategoryId = 2, IsNew = false },
                new Backpack { Id = 9, Name = "APEX IGNITE", Image = "/images/APEX_Ignite.png", Price = 85, Quantity = 11, Rating = 4.8m, CategoryId = 2, IsNew = true },
                new Backpack { Id = 10, Name = "APEX TRANSFORMER", Image = "/images/APEX_Transformer.png", Price = 110, Quantity = 6, SalePrice = 89, Rating = 4.7m, CategoryId = 2, IsNew = false },
                new Backpack { Id = 11, Name = "APEX LEGACY", Image = "/images/APEX_Legacy.png", Price = 85, Quantity = 10, Rating = 3.9m, CategoryId = 2, IsNew = false },

                new Backpack { Id = 12, Name = "APEX ODYSSEY", Image = "/images/APEX_Odyssey.png", Price = 160, Quantity = 3, SalePrice = 130, Rating = 5.0m, CategoryId = 3, IsNew = false },
                new Backpack { Id = 13, Name = "APEX VOYAGER", Image = "/images/APEX_Voyager.png", Price = 145, Quantity = 5, Rating = 5.0m, CategoryId = 3, IsNew = true },
                new Backpack { Id = 14, Name = "APEX SUMMIT", Image = "/images/APEX_Summit.png", Price = 130, Quantity = 4, Rating = 4.7m, CategoryId = 3, IsNew = false },
                new Backpack { Id = 15, Name = "APEX CYBER", Image = "/images/APEX_Cyber.png", Price = 180, Quantity = 2, SalePrice = 149, Rating = 4.6m, CategoryId = 3, IsNew = false }
            );
        }
    }
}
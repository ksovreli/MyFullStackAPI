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
        public DbSet<Review> Reviews { get; set; }
        public DbSet<BackpackImage> BackpackImages { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<PasswordResetCode> PasswordResetCodes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PasswordResetCode>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.HasOne(p => p.User)
                      .WithMany()
                      .HasPrincipalKey(u => u.Email)
                      .HasForeignKey(p => p.Email)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Backpacks" },
                new Category { Id = 2, Name = "Duffel Bags" },
                new Category { Id = 3, Name = "Travel Packs" }
            );

            modelBuilder.Entity<Backpack>().HasData(
                new Backpack { Id = 1, Name = "APEX COMMUTER", Price = 90, Quantity = 10, SalePrice = 65, CategoryId = 1, IsNew = false, Description = "Engineered for the modern explorer, the APEX COMMUTER combines minimalist design with maximum durability." },
                new Backpack { Id = 2, Name = "APEX HERITAGE", Price = 75, Quantity = 12, CategoryId = 1, IsNew = true, Description = "Engineered for the modern explorer, the APEX HERITAGE combines minimalist design with maximum durability." },
                new Backpack { Id = 3, Name = "APEX PULSE", Price = 80, Quantity = 8, CategoryId = 1, IsNew = true, Description = "Engineered for the modern explorer, the APEX PULSE combines minimalist design with maximum durability." },
                new Backpack { Id = 4, Name = "APEX STEALTH", Price = 110, Quantity = 6, SalePrice = 95, CategoryId = 1, IsNew = false, Description = "Engineered for the modern explorer, the APEX STEALTH combines minimalist design with maximum durability." },
                new Backpack { Id = 5, Name = "APEX SKYLINE", Price = 95, Quantity = 9, CategoryId = 1, IsNew = false, Description = "Engineered for the modern explorer, the APEX SKYLINE combines minimalist design with maximum durability." },
                new Backpack { Id = 6, Name = "APEX GLOBAL", Price = 120, Quantity = 5, SalePrice = 85, CategoryId = 1, IsNew = false, Description = "Engineered for the modern explorer, the APEX GLOBAL combines minimalist design with maximum durability." },
                new Backpack { Id = 7, Name = "APEX CROSSOVER", Price = 95, Quantity = 7, CategoryId = 2, IsNew = false, Description = "Engineered for the modern explorer, the APEX CROSSOVER combines minimalist design with maximum durability." },
                new Backpack { Id = 8, Name = "APEX EXECUTIVE", Price = 150, Quantity = 4, SalePrice = 115, CategoryId = 2, IsNew = false, Description = "Engineered for the modern explorer, the APEX EXECUTIVE combines minimalist design with maximum durability." },
                new Backpack { Id = 9, Name = "APEX IGNITE", Price = 85, Quantity = 11, CategoryId = 2, IsNew = true, Description = "Engineered for the modern explorer, the APEX IGNITE combines minimalist design with maximum durability." },
                new Backpack { Id = 10, Name = "APEX TRANSFORMER", Price = 110, Quantity = 6, SalePrice = 89, CategoryId = 2, IsNew = false, Description = "Engineered for the modern explorer, the APEX TRANSFORMER combines minimalist design with maximum durability." },
                new Backpack { Id = 11, Name = "APEX LEGACY", Price = 85, Quantity = 10, CategoryId = 2, IsNew = false, Description = "Engineered for the modern explorer, the APEX LEGACY combines minimalist design with maximum durability." },
                new Backpack { Id = 12, Name = "APEX ODYSSEY", Price = 160, Quantity = 3, SalePrice = 130, CategoryId = 3, IsNew = false, Description = "Engineered for the modern explorer, the APEX ODYSSEY combines minimalist design with maximum durability." },
                new Backpack { Id = 13, Name = "APEX VOYAGER", Price = 145, Quantity = 5, CategoryId = 3, IsNew = true, Description = "Engineered for the modern explorer, the APEX VOYAGER combines minimalist design with maximum durability." },
                new Backpack { Id = 14, Name = "APEX SUMMIT", Price = 130, Quantity = 4, CategoryId = 3, IsNew = false, Description = "Engineered for the modern explorer, the APEX SUMMIT combines minimalist design with maximum durability." },
                new Backpack { Id = 15, Name = "APEX CYBER", Price = 180, Quantity = 2, SalePrice = 149, CategoryId = 3, IsNew = false, Description = "Engineered for the modern explorer, the APEX CYBER combines minimalist design with maximum durability." }
            );

            modelBuilder.Entity<BackpackImage>().HasData(
                new BackpackImage { Id = 1, BackpackId = 1, Url = "/images/APEX_Commuter.png" },
                new BackpackImage { Id = 2, BackpackId = 2, Url = "/images/APEX_Heritage.png" },
                new BackpackImage { Id = 3, BackpackId = 3, Url = "/images/APEX_Pulse.png" },
                new BackpackImage { Id = 4, BackpackId = 4, Url = "/images/APEX_Stealth.png" },
                new BackpackImage { Id = 5, BackpackId = 5, Url = "/images/APEX_Skyline.png" },
                new BackpackImage { Id = 6, BackpackId = 6, Url = "/images/APEX_Global.png" },
                new BackpackImage { Id = 7, BackpackId = 7, Url = "/images/APEX_Crossover.png" },
                new BackpackImage { Id = 8, BackpackId = 8, Url = "/images/APEX_Executive.png" },
                new BackpackImage { Id = 9, BackpackId = 9, Url = "/images/APEX_Ignite.png" },
                new BackpackImage { Id = 10, BackpackId = 10, Url = "/images/APEX_Transformer.png" },
                new BackpackImage { Id = 11, BackpackId = 11, Url = "/images/APEX_Legacy.png" },
                new BackpackImage { Id = 12, BackpackId = 12, Url = "/images/APEX_Odyssey.png" },
                new BackpackImage { Id = 13, BackpackId = 13, Url = "/images/APEX_Voyager.png" },
                new BackpackImage { Id = 14, BackpackId = 14, Url = "/images/APEX_Summit.png" },
                new BackpackImage { Id = 15, BackpackId = 15, Url = "/images/APEX_Cyber.png" }
            );
        }
    }
}
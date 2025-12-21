using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // PHẢI ĐỔI DÒNG NÀY
using Microsoft.EntityFrameworkCore;
using Pawtopia.Models;

namespace Pawtopia.Data
{
    // 1. ĐỔI THÀNH IdentityDbContext<User>
    public class PawtopiaDbContext : IdentityDbContext<User>
    {
        public PawtopiaDbContext(DbContextOptions<PawtopiaDbContext> options)
            : base(options)
        {
        }

        // 2. XÓA dòng public DbSet<User> Users { get; set; } 
        // Vì IdentityDbContext đã tự tạo bảng Users cho bạn rồi.

        public DbSet<Address> Addresses { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Luôn giữ dòng này ở đầu

            // Cấu hình các bảng khác giữ nguyên
            modelBuilder.Entity<Address>().ToTable("Addresses");
            modelBuilder.Entity<Category>().ToTable("Categories");
            modelBuilder.Entity<Product>().ToTable("Products");
            modelBuilder.Entity<Order>().ToTable("Orders");
            modelBuilder.Entity<OrderItem>().ToTable("OrderItems");
            modelBuilder.Entity<ShoppingCart>().ToTable("ShoppingCarts");
            modelBuilder.Entity<ShoppingCartItem>().ToTable("ShoppingCartItems");

            modelBuilder.Entity<Address>()
                .HasOne(a => a.User)
                .WithMany(u => u.Addresses)
                .HasForeignKey(a => a.UserId);
        }
    }
}
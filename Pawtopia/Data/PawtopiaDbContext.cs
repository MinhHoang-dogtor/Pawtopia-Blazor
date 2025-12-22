using Microsoft.EntityFrameworkCore;
using Pawtopia.Models;

namespace Pawtopia.Data
{
    public class PawtopiaDbContext : DbContext
    {
        public PawtopiaDbContext(DbContextOptions<PawtopiaDbContext> options)
            : base(options)
        {
        }

        // --- KHAI BÁO CÁC BẢNG (DbSet) ---
        // Phải có đủ các dòng này thì lệnh Migration mới nhận diện được thay đổi
        public DbSet<User> Users { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình tên bảng thủ công (Nếu muốn khớp chính xác với SQLite hiện tại)
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Address>().ToTable("Addresses");
            modelBuilder.Entity<Category>().ToTable("Categories");
            modelBuilder.Entity<Product>().ToTable("Products");
            modelBuilder.Entity<Order>().ToTable("Orders");
            modelBuilder.Entity<OrderItem>().ToTable("OrderItems");
            modelBuilder.Entity<ShoppingCart>().ToTable("ShoppingCarts");
            modelBuilder.Entity<ShoppingCartItem>().ToTable("ShoppingCartItems");
        }
    }
}
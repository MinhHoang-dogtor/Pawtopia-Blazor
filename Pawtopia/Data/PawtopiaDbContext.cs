using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pawtopia.Models;

namespace Pawtopia.Data
{
    public class PawtopiaDbContext(DbContextOptions<PawtopiaDbContext> options)
        : IdentityDbContext<
            User,
            IdentityRole,
            string, // PasswordHash
            IdentityUserClaim<string>,
            IdentityUserRole<string>,
            IdentityUserLogin<string>,
            IdentityRoleClaim<string>,
            IdentityUserToken<string>,
            IdentityUserPasskey<string>
        >(options)
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ---
            // Customize the ASP.NET Identity models
            // ---

            builder.Entity<User>().ToTable("Users");
            builder.Entity<IdentityRole>().ToTable("Roles");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
            builder.Entity<IdentityUserLogin<string>>().ToTable("Logins");
            builder.Entity<IdentityUserToken<string>>().ToTable("Tokens");
            builder.Entity<IdentityUserPasskey<string>>().ToTable("Passkeys");

            // join table: user can have many roles, role can have many users
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");

            // ---
            // Additional application models
            // ---
        }

        public DbSet<Address> Addresses { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<ProductItem> ProductItems { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderItem> OrderItems { get; set; } = null!;
        public DbSet<ShoppingCart> ShoppingCarts { get; set; } = null!;
        public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; } = null!;
        public DbSet<Variation> Variations { get; set; } = null!;
        public DbSet<VariationOption> VariationOptions { get; set; } = null!;
    }
}

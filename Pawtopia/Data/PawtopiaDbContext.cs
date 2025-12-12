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
            string,
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

            builder.Entity<User>().ToTable("Users");
            builder.Entity<IdentityRole>().ToTable("Roles");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
            builder.Entity<IdentityUserLogin<string>>().ToTable("Logins");
            builder.Entity<IdentityUserToken<string>>().ToTable("Tokens");
            builder.Entity<IdentityUserPasskey<string>>().ToTable("Passkeys");

            // join table: user can have many roles, role can have many users
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
        }
    }
}

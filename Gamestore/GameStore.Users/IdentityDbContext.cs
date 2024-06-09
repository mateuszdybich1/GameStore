using GameStore.Domain.UserEntities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Users;

public class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : IdentityDbContext<PersonModel, RoleModel, Guid>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<PersonModel>().HasKey(p => p.Id);
        builder.Entity<PersonModel>().HasIndex(e => e.Id).IsUnique(true);
        builder.Entity<PersonModel>().Property(p => p.FirstName).IsRequired(false);
        builder.Entity<PersonModel>().Property(p => p.LastName).IsRequired(false);
        builder.Entity<PersonModel>().Property(p => p.NormalizedUserName).IsRequired(false);
        builder.Entity<PersonModel>().Property(p => p.Email).IsRequired(false);
        builder.Entity<PersonModel>().Property(p => p.NormalizedEmail).IsRequired(false);
        builder.Entity<PersonModel>().Property(p => p.IsBanned).HasDefaultValue(false);
        builder.Entity<PersonModel>().Property(p => p.BanDuration).HasDefaultValue(string.Empty);
        builder.Entity<PersonModel>().HasIndex(p => p.UserName).IsUnique();

        builder.Entity<RoleModel>().HasKey(p => p.Id);
        builder.Entity<RoleModel>().HasIndex(p => p.Id).IsUnique();
        builder.Entity<RoleModel>().HasIndex(p => p.Name).IsUnique();
    }
}
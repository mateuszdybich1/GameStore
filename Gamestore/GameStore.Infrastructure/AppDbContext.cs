using GameStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Infrastructure;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Game> Games { get; set; }

    public DbSet<Genre> Genres { get; set; }

    public DbSet<Platform> Platforms { get; set; }

    public DbSet<Publisher> Publishers { get; set; }

    public DbSet<Order> Orders { get; set; }

    public DbSet<OrderGame> OrderGames { get; set; }

    public DbSet<Comment> Comments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Game>().HasKey(e => e.Id);
        modelBuilder.Entity<Game>().HasIndex(e => e.Id).IsUnique(true);
        modelBuilder.Entity<Game>().Property(e => e.Id).IsRequired(true);
        modelBuilder.Entity<Game>().Property(e => e.Name).IsRequired(true);
        modelBuilder.Entity<Game>().HasIndex(e => e.Key).IsUnique(true);
        modelBuilder.Entity<Game>().Property(e => e.Key).IsRequired(true);
        modelBuilder.Entity<Game>().Property(e => e.Price).IsRequired(true);
        modelBuilder.Entity<Game>().Property(e => e.UnitInStock).IsRequired(true);
        modelBuilder.Entity<Game>().Property(e => e.Discount).IsRequired(true);
        modelBuilder.Entity<Game>().Property(e => e.Description).IsRequired(false);

        modelBuilder.Entity<Genre>().HasKey(e => e.Id);
        modelBuilder.Entity<Genre>().HasIndex(e => e.Id).IsUnique(true);
        modelBuilder.Entity<Genre>().Property(e => e.Id).IsRequired(true);
        modelBuilder.Entity<Genre>().Property(e => e.Name).IsRequired(true);
        modelBuilder.Entity<Genre>().HasIndex(e => e.Name).IsUnique(true);

        modelBuilder.Entity<Platform>().HasKey(e => e.Id);
        modelBuilder.Entity<Platform>().HasIndex(e => e.Id).IsUnique(true);
        modelBuilder.Entity<Platform>().Property(e => e.Id).IsRequired(true);
        modelBuilder.Entity<Platform>().Property(e => e.Type).IsRequired(true);
        modelBuilder.Entity<Platform>().HasIndex(e => e.Type).IsUnique(true);

        modelBuilder.Entity<Publisher>().HasKey(e => e.Id);
        modelBuilder.Entity<Publisher>().HasIndex(e => e.Id).IsUnique(true);
        modelBuilder.Entity<Publisher>().Property(e => e.Id).IsRequired(true);
        modelBuilder.Entity<Publisher>().Property(e => e.CompanyName).IsRequired(true);
        modelBuilder.Entity<Publisher>().HasIndex(e => e.CompanyName).IsUnique(true);
        modelBuilder.Entity<Publisher>().Property(e => e.HomePage).IsRequired(false);
        modelBuilder.Entity<Publisher>().Property(e => e.Description).IsRequired(false);

        modelBuilder.Entity<Order>().HasKey(e => e.Id);
        modelBuilder.Entity<Order>().HasIndex(e => e.Id).IsUnique(true);
        modelBuilder.Entity<Order>().Property(e => e.Id).IsRequired(true);
        modelBuilder.Entity<Order>().Property(e => e.CustomerId).IsRequired(true);
        modelBuilder.Entity<Order>().Property(e => e.Status).IsRequired(true);

        modelBuilder.Entity<OrderGame>().HasKey(e => new { e.OrderId, e.ProductId });
        modelBuilder.Entity<OrderGame>().HasIndex(e => new { e.OrderId, e.ProductId }).IsUnique(true);
        modelBuilder.Entity<OrderGame>().Property(e => e.OrderId).IsRequired(true);
        modelBuilder.Entity<OrderGame>().Property(e => e.ProductId).IsRequired(true);
        modelBuilder.Entity<OrderGame>().Property(e => e.Price).IsRequired(true);
        modelBuilder.Entity<OrderGame>().Property(e => e.Quantity).IsRequired(true);

        modelBuilder.Entity<Comment>().HasKey(e => e.Id);
        modelBuilder.Entity<Comment>().HasIndex(e => e.Id).IsUnique(true);
        modelBuilder.Entity<Comment>().Property(e => e.Id).IsRequired(true);
        modelBuilder.Entity<Comment>().Property(e => e.Name).IsRequired(true);
        modelBuilder.Entity<Comment>().Property(e => e.Body).IsRequired(true);

        modelBuilder.Entity<Game>().HasMany(e => e.Genres).WithMany(e => e.Games);
        modelBuilder.Entity<Game>().HasMany(e => e.Platforms).WithMany(e => e.Games);
        modelBuilder.Entity<Publisher>().HasMany(e => e.Games).WithOne(e => e.Publisher).HasForeignKey(e => e.PublisherId).IsRequired(true).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Game>().HasMany(e => e.Comments).WithOne(e => e.Game).OnDelete(DeleteBehavior.Cascade);
    }
}
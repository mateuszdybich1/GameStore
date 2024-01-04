﻿using GameStore.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Infrastructure;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Game> Games { get; set; }

    public DbSet<Genre> Genres { get; set; }

    public DbSet<Platform> Platforms { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Game>().HasKey(e => e.Id);
        modelBuilder.Entity<Game>().HasIndex(e => e.Id).IsUnique(true);
        modelBuilder.Entity<Game>().Property(e => e.Id).IsRequired(true);
        modelBuilder.Entity<Game>().Property(e => e.Name).IsRequired(true);
        modelBuilder.Entity<Game>().HasIndex(x => x.Key).IsUnique(true);
        modelBuilder.Entity<Game>().Property(e => e.Key).IsRequired(true);

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

        modelBuilder.Entity<Game>().HasMany(x => x.Genres).WithMany(x => x.Games);
        modelBuilder.Entity<Game>().HasMany(x => x.Platforms).WithMany(x => x.Games);
    }
}
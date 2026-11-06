using System;
using System.Collections.Generic;
using AromaDom.Models;
using Microsoft.EntityFrameworkCore;

namespace AromaDom.Data;

public partial class AromaDomDevContext : DbContext
{
    public AromaDomDevContext()
    {
    }

    public AromaDomDevContext(DbContextOptions<AromaDomDevContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Material> Materials { get; set; }

    public virtual DbSet<MaterialProduct> MaterialProducts { get; set; }

    public virtual DbSet<MaterialType> MaterialTypes { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductType> ProductTypes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(
                "Server=localhost;Database=AromaDomDev;Trusted_Connection=True;TrustServerCertificate=True");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Material>(entity =>
        {
            entity.HasKey(e => e.MaterialId).HasName("PK__Material__C50610F795820643");

            entity.HasIndex(e => e.Name, "UQ__Material__737584F6BB0084FC").IsUnique();

            entity.Property(e => e.MinQuantity).HasColumnType("decimal(12, 3)");
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.PackageQuantity).HasColumnType("decimal(12, 3)");
            entity.Property(e => e.Price).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.StockQuantity).HasColumnType("decimal(12, 3)");
            entity.Property(e => e.Unit).HasMaxLength(30);

            entity.HasOne(d => d.MaterialType).WithMany(p => p.Materials)
                .HasForeignKey(d => d.MaterialTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Materials__Mater__6A30C649");
        });

        modelBuilder.Entity<MaterialProduct>(entity =>
        {
            entity.HasKey(e => new { e.MaterialId, e.ProductId }).HasName("PK__Material__0E46DC9B92761CCE");

            entity.Property(e => e.MaterialQuantity).HasColumnType("decimal(12, 3)");

            entity.HasOne(d => d.Material).WithMany(p => p.MaterialProducts)
                .HasForeignKey(d => d.MaterialId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MaterialP__Mater__73BA3083");

            entity.HasOne(d => d.Product).WithMany(p => p.MaterialProducts)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MaterialP__Produ__74AE54BC");
        });

        modelBuilder.Entity<MaterialType>(entity =>
        {
            entity.HasKey(e => e.MaterialTypeId).HasName("PK__Material__1621BBDFECBA93C4");

            entity.HasIndex(e => e.Name, "UQ__Material__737584F6D2089678").IsUnique();

            entity.Property(e => e.LossPercent).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Products__B40CC6CDC3EF9AD1");

            entity.HasIndex(e => e.Article, "UQ__Products__4943444A1603E9EE").IsUnique();

            entity.HasIndex(e => e.Name, "UQ__Products__737584F6ACCBECEB").IsUnique();

            entity.Property(e => e.Article).HasMaxLength(30);
            entity.Property(e => e.MinPartnerPrice).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.Name).HasMaxLength(200);

            entity.HasOne(d => d.ProductType).WithMany(p => p.Products)
                .HasForeignKey(d => d.ProductTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Products__Produc__6FE99F9F");
        });

        modelBuilder.Entity<ProductType>(entity =>
        {
            entity.HasKey(e => e.ProductTypeId).HasName("PK__ProductT__A1312F6EB6D041E0");

            entity.HasIndex(e => e.Name, "UQ__ProductT__737584F6A9C8E609").IsUnique();

            entity.Property(e => e.Coefficient).HasColumnType("decimal(8, 2)");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

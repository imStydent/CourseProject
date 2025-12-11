using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ScheduleApp.Models;

namespace ScheduleApp.Data;

public partial class Context : DbContext
{
    public Context()
    {
    }

    public Context(DbContextOptions<Context> options)
        : base(options)
    {
    }

    public virtual DbSet<LoadUnloadOperation> LoadUnloadOperations { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductsHasOrder> ProductsHasOrders { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Worker> Workers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySQL("server=mysql;database=ispp2102;user id=ispp2102;password=2102");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LoadUnloadOperation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("load_unload_operations");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Date)
                .HasColumnType("datetime")
                .HasColumnName("date");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("orders");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.LoadTime)
                .HasColumnType("time")
                .HasColumnName("load_time");
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .HasColumnName("name");
            entity.Property(e => e.UnloadTime)
                .HasColumnType("time")
                .HasColumnName("unload_time");

            entity.HasMany(d => d.LoadUnloadOpertations).WithMany(p => p.Orders)
                .UsingEntity<Dictionary<string, object>>(
                    "OrdersHasLoadUnloadOpertation",
                    r => r.HasOne<LoadUnloadOperation>().WithMany()
                        .HasForeignKey("LoadUnloadOpertationsId")
                        .HasConstraintName("fk_Orders_has_load_unload_opertations_load_unload_opertations1"),
                    l => l.HasOne<Order>().WithMany()
                        .HasForeignKey("OrdersId")
                        .HasConstraintName("fk_Orders_has_load_unload_opertations_Orders1"),
                    j =>
                    {
                        j.HasKey("OrdersId", "LoadUnloadOpertationsId").HasName("PRIMARY");
                        j.ToTable("orders_has_load_unload_opertations");
                        j.HasIndex(new[] { "OrdersId" }, "fk_Orders_has_load_unload_opertations_Orders1_idx");
                        j.HasIndex(new[] { "LoadUnloadOpertationsId" }, "fk_Orders_has_load_unload_opertations_load_unload_opertatio_idx");
                        j.IndexerProperty<int>("OrdersId").HasColumnName("Orders_id");
                        j.IndexerProperty<int>("LoadUnloadOpertationsId").HasColumnName("load_unload_opertations_id");
                    });
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("products");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Kind)
                .HasColumnType("enum('Термоупаковка','Ящик тетра-крейт','Село Устьяны','Радостино')")
                .HasColumnName("kind");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.PiecesPerBox).HasColumnName("pieces_per_box");
        });

        modelBuilder.Entity<ProductsHasOrder>(entity =>
        {
            entity.HasKey(e => new { e.ProductsId, e.OrdersId }).HasName("PRIMARY");

            entity.ToTable("products_has_orders");

            entity.HasIndex(e => e.OrdersId, "fk_Products_has_Orders_Orders1_idx");

            entity.HasIndex(e => e.ProductsId, "fk_Products_has_Orders_Products1_idx");

            entity.Property(e => e.ProductsId).HasColumnName("Products_id");
            entity.Property(e => e.OrdersId).HasColumnName("Orders_id");
            entity.Property(e => e.Amount).HasColumnName("amount");

            entity.HasOne(d => d.Orders).WithMany(p => p.ProductsHasOrders)
                .HasForeignKey(d => d.OrdersId)
                .HasConstraintName("fk_Products_has_Orders_Orders1");

            entity.HasOne(d => d.Products).WithMany(p => p.ProductsHasOrders)
                .HasForeignKey(d => d.ProductsId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Products_has_Orders_Products1");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("roles");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Worker>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("workers");

            entity.HasIndex(e => e.RoleId, "fk_Workers_Roles_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(60)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(8)
                .HasColumnName("password");
            entity.Property(e => e.Patronymic)
                .HasMaxLength(60)
                .HasColumnName("patronymic");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(100)
                .HasColumnName("phone_number");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Surname)
                .HasMaxLength(60)
                .HasColumnName("surname");

            entity.HasOne(d => d.Role).WithMany(p => p.Workers)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Workers_Roles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

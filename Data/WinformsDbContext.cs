using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ProyectoConsolaObjetos1.Models;

namespace ProyectoConsolaObjetos1.Data;

public partial class WinformsDbContext : DbContext
{
    public WinformsDbContext(DbContextOptions<WinformsDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Detallesventum> Detallesventa { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<Venta> Ventas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_general_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Detallesventum>(entity =>
        {
            entity.HasKey(e => new { e.VentaCodigo, e.ProductoCodigo })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("detallesventa");

            entity.HasIndex(e => e.ProductoCodigo, "IX_DetallesVenta_ProductoCodigo");

            entity.Property(e => e.VentaCodigo).HasColumnType("int(11)");
            entity.Property(e => e.ProductoCodigo).HasColumnType("int(11)");
            entity.Property(e => e.Cantidad).HasColumnType("int(11)");
            entity.Property(e => e.PrecioUnitario).HasPrecision(18, 2);

            entity.HasOne(d => d.ProductoCodigoNavigation).WithMany(p => p.Detallesventa)
                .HasForeignKey(d => d.ProductoCodigo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetallesVenta_Productos_ProductoCodigo");

            entity.HasOne(d => d.VentaCodigoNavigation).WithMany(p => p.Detallesventa)
                .HasForeignKey(d => d.VentaCodigo)
                .HasConstraintName("FK_DetallesVenta_Ventas_VentaCodigo");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.Codigo).HasName("PRIMARY");

            entity.ToTable("productos");

            entity.HasIndex(e => new { e.Nombre, e.Categoria }, "IX_Productos_Nombre_Categoria");

            entity.Property(e => e.Codigo)
                .ValueGeneratedNever()
                .HasColumnType("int(11)");
            entity.Property(e => e.Categoria).HasMaxLength(100);
            entity.Property(e => e.Descripcion).HasMaxLength(500);
            entity.Property(e => e.Nombre).HasMaxLength(150);
            entity.Property(e => e.PrecioVenta).HasPrecision(18, 2);
            entity.Property(e => e.StockActual).HasColumnType("int(11)");
            entity.Property(e => e.StockMinimo).HasColumnType("int(11)");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("usuarios");

            entity.HasIndex(e => e.Codigo, "IX_Usuarios_Codigo").IsUnique();

            entity.HasIndex(e => e.Correo, "IX_Usuarios_Correo").IsUnique();

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.Clave).HasMaxLength(256);
            entity.Property(e => e.Codigo).HasColumnType("int(11)");
            entity.Property(e => e.Correo).HasMaxLength(250);
            entity.Property(e => e.Direccion).HasMaxLength(300);
            entity.Property(e => e.Nombre).HasMaxLength(150);
            entity.Property(e => e.Rol).HasColumnType("int(11)");
        });

        modelBuilder.Entity<Venta>(entity =>
        {
            entity.HasKey(e => e.Codigo).HasName("PRIMARY");

            entity.ToTable("ventas");

            entity.HasIndex(e => e.ClienteId, "IX_Ventas_ClienteId");

            entity.HasIndex(e => e.EmpleadoId, "IX_Ventas_EmpleadoId");

            entity.Property(e => e.Codigo)
                .ValueGeneratedNever()
                .HasColumnType("int(11)");
            entity.Property(e => e.ClienteId).HasColumnType("int(11)");
            entity.Property(e => e.EmpleadoId).HasColumnType("int(11)");
            entity.Property(e => e.FechaVenta).HasMaxLength(6);

            entity.HasOne(d => d.Cliente).WithMany(p => p.VentaClientes)
                .HasForeignKey(d => d.ClienteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ventas_Usuarios_ClienteId");

            entity.HasOne(d => d.Empleado).WithMany(p => p.VentaEmpleados)
                .HasForeignKey(d => d.EmpleadoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ventas_Usuarios_EmpleadoId");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

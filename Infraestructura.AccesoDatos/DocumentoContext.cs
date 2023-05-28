using Entidades.Entidades;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infraestructura.AccesoDatos
{
    public class DocumentoContext : IdentityDbContext<UsuarioApp, RolApp, Guid, UsuarioNotificacion, UsuarioRol, UsuarioLogin, RoleNotificacion, UsuarioToken>
    {
        public DocumentoContext()
        {
        }

        public DocumentoContext(DbContextOptions<DocumentoContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=MiProyectoCoreDB;Trusted_Connection=True;MultipleActiveResultSets=true");
        }

        public DbSet<Area> Areas { get; set; }
        public DbSet<Documento> Documentos { get; set; }
        public DbSet<LogTransaccion> LogTransacciones { get; set; }
        public DbSet<Renta> Rentas { get; set; }
        public DbSet<Personal> Personals { get; set; }
        public DbSet<Observacion> Observacions { get; set; }
        public DbSet<Departamento> Departamentos { get; set; }
        public DbSet<Barrio> Barrios { get; set; }
        public DbSet<Municipio> Municipios { get; set; }
        public DbSet<Oficina> Oficinas { get; set; }
        public DbSet<Actividad> Actividads { get; set; }
        public DbSet<DetalleActividad> detalleActividads { get; set; }
        public DbSet<Cuenta> Cuentas { get; set; }
        public DbSet<TipoMovimiento> TipoMovimientos { get; set; }
        public DbSet<Movimiento> Movimientos { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<UsuarioApp>().ToTable("Usuarios", "SEG");
            builder.Entity<RolApp>().ToTable("Roles", "SEG");
            builder.Entity<UsuarioNotificacion>().ToTable("UsuariosNotificaciones", "SEG");
            builder.Entity<UsuarioRol>().ToTable("UsuariosRoles", "SEG");
            builder.Entity<UsuarioLogin>().ToTable("UsuariosLogin", "SEG");
            builder.Entity<RoleNotificacion>().ToTable("RolesNotificaciones", "SEG");
            builder.Entity<UsuarioToken>().ToTable("UsuariosToken", "SEG");

            builder.Entity<Area>().ToTable("Area", "DOC");
            builder.Entity<Documento>().ToTable("Documento", "DOC");

            builder.Entity<Renta>().ToTable("Renta", "JS");
            builder.Entity<Personal>().ToTable("Personal", "JS");
            builder.Entity<Observacion>().ToTable("Observacion", "JS");
            builder.Entity<Departamento>().ToTable("Departamento", "JS");
            builder.Entity<Municipio>().ToTable("Municipio", "JS");
            builder.Entity<Barrio>().ToTable("Barrio", "JS");
            builder.Entity<Oficina>().ToTable("Oficina", "JS");
            builder.Entity<Actividad>().ToTable("Actividad", "JS");
            builder.Entity<DetalleActividad>().ToTable("DetalleActividad", "JS");

            builder.Entity<Cuenta>().ToTable("Cuenta", "PER");
            builder.Entity<TipoMovimiento>().ToTable("TipoMovimiento", "PER");
            builder.Entity<Movimiento>().ToTable("Movimiento", "PER");
        }
    }
}

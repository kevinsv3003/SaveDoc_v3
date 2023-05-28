﻿// <auto-generated />
using System;
using Infraestructura.AccesoDatos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infraestructura.AccesoDatos.Migrations
{
    [DbContext(typeof(DocumentoContext))]
    [Migration("20220824154501_barrioCodMigra")]
    partial class barrioCodMigra
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.15")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Entidades.Entidades.Actividad", b =>
                {
                    b.Property<int>("ActividadId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Descripcion")
                        .HasColumnType("varchar(200)");

                    b.Property<bool>("EsEspecial")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("Fecha")
                        .IsRequired()
                        .HasColumnType("datetime2");

                    b.Property<string>("Lugar")
                        .IsRequired()
                        .HasColumnType("varchar(200)");

                    b.Property<string>("NombreActividad")
                        .IsRequired()
                        .HasColumnType("varchar(200)");

                    b.HasKey("ActividadId");

                    b.ToTable("Actividad", "JS");
                });

            modelBuilder.Entity("Entidades.Entidades.Area", b =>
                {
                    b.Property<int>("AreaId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Descripcion")
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<bool>("Estado")
                        .HasColumnType("bit");

                    b.Property<byte[]>("Fondo")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)");

                    b.HasKey("AreaId");

                    b.ToTable("Area", "DOC");
                });

            modelBuilder.Entity("Entidades.Entidades.Barrio", b =>
                {
                    b.Property<int>("BarrioId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("MunicipioId")
                        .IsRequired()
                        .HasColumnType("int");

                    b.Property<string>("NombreBarrio")
                        .IsRequired()
                        .HasColumnType("varchar(200)");

                    b.HasKey("BarrioId");

                    b.HasIndex("MunicipioId");

                    b.ToTable("Barrio", "JS");
                });

            modelBuilder.Entity("Entidades.Entidades.Departamento", b =>
                {
                    b.Property<int>("DepartamentoId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("DescripcionDepartamento")
                        .IsRequired()
                        .HasColumnType("varchar(200)");

                    b.HasKey("DepartamentoId");

                    b.ToTable("Departamento", "JS");
                });

            modelBuilder.Entity("Entidades.Entidades.DetalleActividad", b =>
                {
                    b.Property<int>("DetalleActividadId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ActividadId")
                        .HasColumnType("int");

                    b.Property<bool>("Asistio")
                        .HasColumnType("bit");

                    b.Property<string>("Observacion")
                        .HasColumnType("varchar(200)");

                    b.Property<int>("PersonalId")
                        .HasColumnType("int");

                    b.Property<bool>("Transporte")
                        .HasColumnType("bit");

                    b.HasKey("DetalleActividadId");

                    b.HasIndex("ActividadId");

                    b.HasIndex("PersonalId");

                    b.ToTable("DetalleActividad", "JS");
                });

            modelBuilder.Entity("Entidades.Entidades.Documento", b =>
                {
                    b.Property<int>("DocumentoId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("AreaId")
                        .IsRequired()
                        .HasColumnType("int");

                    b.Property<string>("Descripcion")
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<string>("Extension")
                        .HasColumnType("varchar(200)");

                    b.Property<DateTime?>("FechaRegistro")
                        .HasColumnType("datetime");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.HasKey("DocumentoId");

                    b.HasIndex("AreaId");

                    b.ToTable("Documento", "DOC");
                });

            modelBuilder.Entity("Entidades.Entidades.LogTransaccion", b =>
                {
                    b.Property<int>("LogTransaccionID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Accion")
                        .IsRequired()
                        .HasColumnType("varchar(200)");

                    b.Property<DateTime?>("FechaProceso")
                        .IsRequired()
                        .HasColumnType("datetime2");

                    b.Property<string>("Usuario")
                        .IsRequired()
                        .HasColumnType("varchar(200)");

                    b.HasKey("LogTransaccionID");

                    b.ToTable("LogTransaccion", "AUDI");
                });

            modelBuilder.Entity("Entidades.Entidades.Municipio", b =>
                {
                    b.Property<int>("MunicipioId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("DepartamentoId")
                        .IsRequired()
                        .HasColumnType("int");

                    b.Property<string>("DescripcionMunicipio")
                        .IsRequired()
                        .HasColumnType("varchar(200)");

                    b.HasKey("MunicipioId");

                    b.HasIndex("DepartamentoId");

                    b.ToTable("Municipio", "JS");
                });

            modelBuilder.Entity("Entidades.Entidades.Observacion", b =>
                {
                    b.Property<int>("ObservacionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ObservacionDet")
                        .IsRequired()
                        .HasColumnType("varchar(200)");

                    b.Property<int?>("PersonalId")
                        .IsRequired()
                        .HasColumnType("int");

                    b.HasKey("ObservacionId");

                    b.HasIndex("PersonalId");

                    b.ToTable("Observacion", "JS");
                });

            modelBuilder.Entity("Entidades.Entidades.Oficina", b =>
                {
                    b.Property<int>("OficinaId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Descripcion")
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<bool>("Estado")
                        .HasColumnType("bit");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)");

                    b.HasKey("OficinaId");

                    b.ToTable("Oficina", "JS");
                });

            modelBuilder.Entity("Entidades.Entidades.Personal", b =>
                {
                    b.Property<int>("PersonalId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Cedula")
                        .IsRequired()
                        .HasColumnType("varchar(20)");

                    b.Property<int?>("CodBarrio")
                        .HasColumnType("int");

                    b.Property<int>("CodDepartamento")
                        .HasColumnType("int");

                    b.Property<int>("CodMunicipio")
                        .HasColumnType("int");

                    b.Property<string>("Direccion")
                        .IsRequired()
                        .HasColumnType("varchar(200)");

                    b.Property<bool>("Disponible")
                        .HasColumnType("bit");

                    b.Property<string>("Genero")
                        .IsRequired()
                        .HasColumnType("varchar(15)");

                    b.Property<int?>("OficinaId")
                        .IsRequired()
                        .HasColumnType("int");

                    b.Property<string>("PrimerApellido")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.Property<string>("PrimerNombre")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.Property<int?>("RentaId")
                        .IsRequired()
                        .HasColumnType("int");

                    b.Property<string>("SegundoApellido")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.Property<string>("SegundoNombre")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.Property<string>("Telefono")
                        .IsRequired()
                        .HasColumnType("varchar(10)");

                    b.HasKey("PersonalId");

                    b.HasIndex("OficinaId");

                    b.HasIndex("RentaId");

                    b.ToTable("Personal", "JS");
                });

            modelBuilder.Entity("Entidades.Entidades.Renta", b =>
                {
                    b.Property<int>("RentaId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Descripcion")
                        .HasColumnType("varchar(200)");

                    b.Property<string>("NombreRenta")
                        .IsRequired()
                        .HasColumnType("varchar(200)");

                    b.HasKey("RentaId");

                    b.ToTable("Renta", "JS");
                });

            modelBuilder.Entity("Entidades.Entidades.RolApp", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Descripcion")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Descripcion");

                    b.Property<bool>("EsBorrado")
                        .HasColumnType("bit")
                        .HasColumnName("EsBorrado");

                    b.Property<int>("LogId")
                        .HasColumnType("int")
                        .HasColumnName("LogId");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<int>("UsuarioLogId")
                        .HasColumnType("int")
                        .HasColumnName("UsuarioLogId");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("Roles", "SEG");
                });

            modelBuilder.Entity("Entidades.Entidades.RoleNotificacion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("EsBorrado")
                        .HasColumnType("bit")
                        .HasColumnName("EsBorrado");

                    b.Property<int>("LogId")
                        .HasColumnType("int")
                        .HasColumnName("LogId");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("UsuarioLogId")
                        .HasColumnType("int")
                        .HasColumnName("UsuarioLogId");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("RolesNotificaciones", "SEG");
                });

            modelBuilder.Entity("Entidades.Entidades.UsuarioApp", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("Apellidos")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Direccion")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Edad")
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<DateTime>("FechaNacimiento")
                        .HasColumnType("datetime2");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Nombres")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Sexo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("Usuarios", "SEG");
                });

            modelBuilder.Entity("Entidades.Entidades.UsuarioLogin", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("EsBorrado")
                        .HasColumnType("bit")
                        .HasColumnName("EsBorrado");

                    b.Property<int>("LogId")
                        .HasColumnType("int")
                        .HasColumnName("LogId");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("UsuarioLogId")
                        .HasColumnType("int")
                        .HasColumnName("UsuarioLogId");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("UsuariosLogin", "SEG");
                });

            modelBuilder.Entity("Entidades.Entidades.UsuarioNotificacion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("EsBorrado")
                        .HasColumnType("bit")
                        .HasColumnName("EsBorrado");

                    b.Property<int>("LogId")
                        .HasColumnType("int")
                        .HasColumnName("LogId");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("UsuarioLogId")
                        .HasColumnType("int")
                        .HasColumnName("UsuarioLogId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UsuariosNotificaciones", "SEG");
                });

            modelBuilder.Entity("Entidades.Entidades.UsuarioRol", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("EsBorrado")
                        .HasColumnType("bit")
                        .HasColumnName("EsBorrado");

                    b.Property<int>("LogId")
                        .HasColumnType("int")
                        .HasColumnName("LogId");

                    b.Property<int>("UsuarioLogId")
                        .HasColumnType("int")
                        .HasColumnName("UsuarioLogId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("UsuariosRoles", "SEG");
                });

            modelBuilder.Entity("Entidades.Entidades.UsuarioToken", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("EsBorrado")
                        .HasColumnType("bit")
                        .HasColumnName("EsBorrado");

                    b.Property<int>("LogId")
                        .HasColumnType("int")
                        .HasColumnName("LogId");

                    b.Property<int>("UsuarioLogId")
                        .HasColumnType("int")
                        .HasColumnName("UsuarioLogId");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("UsuariosToken", "SEG");
                });

            modelBuilder.Entity("Entidades.Entidades.Barrio", b =>
                {
                    b.HasOne("Entidades.Entidades.Municipio", "Municipios")
                        .WithMany()
                        .HasForeignKey("MunicipioId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Municipios");
                });

            modelBuilder.Entity("Entidades.Entidades.DetalleActividad", b =>
                {
                    b.HasOne("Entidades.Entidades.Actividad", "Actividad")
                        .WithMany()
                        .HasForeignKey("ActividadId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Entidades.Entidades.Personal", "Personal")
                        .WithMany()
                        .HasForeignKey("PersonalId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Actividad");

                    b.Navigation("Personal");
                });

            modelBuilder.Entity("Entidades.Entidades.Documento", b =>
                {
                    b.HasOne("Entidades.Entidades.Area", "Areas")
                        .WithMany()
                        .HasForeignKey("AreaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Areas");
                });

            modelBuilder.Entity("Entidades.Entidades.Municipio", b =>
                {
                    b.HasOne("Entidades.Entidades.Departamento", "Departamentos")
                        .WithMany()
                        .HasForeignKey("DepartamentoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Departamentos");
                });

            modelBuilder.Entity("Entidades.Entidades.Observacion", b =>
                {
                    b.HasOne("Entidades.Entidades.Personal", "Personals")
                        .WithMany()
                        .HasForeignKey("PersonalId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Personals");
                });

            modelBuilder.Entity("Entidades.Entidades.Personal", b =>
                {
                    b.HasOne("Entidades.Entidades.Oficina", "Oficinas")
                        .WithMany()
                        .HasForeignKey("OficinaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Entidades.Entidades.Renta", "Rentas")
                        .WithMany()
                        .HasForeignKey("RentaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Oficinas");

                    b.Navigation("Rentas");
                });

            modelBuilder.Entity("Entidades.Entidades.RoleNotificacion", b =>
                {
                    b.HasOne("Entidades.Entidades.RolApp", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Entidades.Entidades.UsuarioLogin", b =>
                {
                    b.HasOne("Entidades.Entidades.UsuarioApp", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Entidades.Entidades.UsuarioNotificacion", b =>
                {
                    b.HasOne("Entidades.Entidades.UsuarioApp", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Entidades.Entidades.UsuarioRol", b =>
                {
                    b.HasOne("Entidades.Entidades.RolApp", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Entidades.Entidades.UsuarioApp", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Entidades.Entidades.UsuarioToken", b =>
                {
                    b.HasOne("Entidades.Entidades.UsuarioApp", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}

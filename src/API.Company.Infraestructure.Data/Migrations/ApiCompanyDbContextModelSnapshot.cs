﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using API.Company.Infraestructure.Data;

namespace API.Company.Infraestructure.Data.Migrations
{
    [DbContext(typeof(ApiCompanyDbContext))]
    partial class ApiCompanyDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.32")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("API.Company.Domain.Entities.Applications", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Fxaltareg")
                        .HasColumnName("fxaltareg")
                        .HasColumnType("datetime");

                    b.Property<DateTime>("Fxmodreg")
                        .HasColumnName("fxmodreg")
                        .HasColumnType("datetime");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)")
                        .HasMaxLength(50);

                    b.Property<int>("SourceUpdate")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Applications");
                });

            modelBuilder.Entity("API.Company.Domain.Entities.Organizations", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CadenaConexion")
                        .IsRequired()
                        .HasColumnType("varchar(MAX)");

                    b.Property<string>("Entorno")
                        .IsRequired()
                        .HasColumnType("nvarchar(10)")
                        .HasMaxLength(10);

                    b.Property<string>("Estado")
                        .IsRequired()
                        .HasColumnType("nvarchar(20)")
                        .HasMaxLength(20);

                    b.Property<DateTime>("Fxaltareg")
                        .HasColumnName("fxaltareg")
                        .HasColumnType("datetime");

                    b.Property<DateTime>("Fxmodreg")
                        .HasColumnName("fxmodreg")
                        .HasColumnType("datetime");

                    b.Property<int>("IdApplication")
                        .HasColumnType("int");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)")
                        .HasMaxLength(50);

                    b.Property<string>("Version")
                        .IsRequired()
                        .HasColumnType("nvarchar(20)")
                        .HasMaxLength(20);

                    b.HasKey("Id");

                    b.HasIndex("IdApplication");

                    b.HasIndex("Nombre", "IdApplication")
                        .IsUnique();

                    b.ToTable("Organizations");
                });

            modelBuilder.Entity("API.Company.Domain.Entities.Procesos", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("IdOrganizacion")
                        .HasColumnType("int");

                    b.Property<string>("Obs")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Proceso")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)")
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.HasIndex("IdOrganizacion");

                    b.ToTable("Procesos");
                });

            modelBuilder.Entity("API.Company.Domain.Entities.Organizations", b =>
                {
                    b.HasOne("API.Company.Domain.Entities.Applications", "IdApplicationNavigation")
                        .WithMany("Organizations")
                        .HasForeignKey("IdApplication")
                        .HasConstraintName("FK_Organizations_Applications")
                        .IsRequired();
                });

            modelBuilder.Entity("API.Company.Domain.Entities.Procesos", b =>
                {
                    b.HasOne("API.Company.Domain.Entities.Organizations", "IdOrganizacionNavigation")
                        .WithMany("Procesos")
                        .HasForeignKey("IdOrganizacion")
                        .HasConstraintName("FK_Procesos_Organizations")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
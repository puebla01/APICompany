﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Moinsa.Arcante.Company.Domain.Entities;
using Moinsa.Arcante.Company.Infraestructure.Data;
using System;
using System.Collections.Generic;

namespace Moinsa.Arcante.Company.Infraestructure.Data.Configurations
{
    public partial class OrganizationsConfiguration : IEntityTypeConfiguration<Organizations>
    {
        public void Configure(EntityTypeBuilder<Organizations> entity)
        {
            entity.HasIndex(e => new { e.Nombre, e.IdApplication })
                .IsUnique();

            entity.Property(e => e.CadenaConexion).IsRequired();

            entity.Property(e => e.Entorno).HasMaxLength(10);

            entity.Property(e => e.Estado).HasMaxLength(20);

            entity.Property(e => e.Fxaltareg)
                .HasColumnType("datetime")
                .HasColumnName("fxaltareg");

            entity.Property(e => e.Fxmodreg)
                .HasColumnType("datetime")
                .HasColumnName("fxmodreg");

            entity.Property(e => e.Nombre)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.Version).HasMaxLength(20);

            entity.HasOne(d => d.IdApplicationNavigation)
                .WithMany(p => p.Organizations)
                .HasForeignKey(d => d.IdApplication)
                .IsRequired()
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Organizations_Applications");

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<Organizations> entity);
    }
}

﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using API.Company.Domain.Entities;
using API.Company.Infraestructure.Data;
using System;
using System.Collections.Generic;

namespace API.Company.Infraestructure.Data.Configurations
{
    public partial class ProcesosConfiguration : IEntityTypeConfiguration<Procesos>
    {
        public void Configure(EntityTypeBuilder<Procesos> entity)
        {
            entity.Property(e => e.Proceso)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasOne(d => d.IdOrganizacionNavigation)
                .WithMany(p => p.Procesos)
                .HasForeignKey(d => d.IdOrganizacion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Procesos_Organizations");

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<Procesos> entity);
    }
}

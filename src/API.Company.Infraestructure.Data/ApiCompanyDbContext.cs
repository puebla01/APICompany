
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using API.Company.Domain.Entities;
using API.Company.Infraestructure.Data.Configurations;
using System;
using System.Collections.Generic;
namespace API.Company.Infraestructure.Data
{
    public partial class ApiCompanyDbContext 
    {
        public virtual DbSet<Applications> Applications { get; set; }
        public virtual DbSet<Organizations> Organizations { get; set; }
        public virtual DbSet<Procesos> Procesos { get; set; }

        public ApiCompanyDbContext()
        {
        }

        public ApiCompanyDbContext(DbContextOptions<ApiCompanyDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        modelBuilder.ApplyConfiguration(new Configurations.ApplicationsConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.OrganizationsConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.ProcesosConfiguration());

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

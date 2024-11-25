using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moinsa.Arcante.Company.Business.Repositories.Interfaces;
using Moinsa.Arcante.Company.Business.Validations;
using Moinsa.Arcante.Company.Domain.Entities;
using Moinsa.Arcante.Company.Domain.Enums;
using Moinsa.Arcante.Company.Infraestructure.Data;
using Moinsa.Arcante.Company.Models.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Moinsa.Arcante.Company.Business.Repositories
{
    public class ApplicationsRepository : BaseRepository, IApplicationsRepository
    {

        public ApplicationsRepository(ApiCompanyDbContext dbContext,
                                    ILogger<ApplicationsRepository> logger,
                                    IMapper mapper
                                    ) : base(dbContext, logger, mapper)
        {

        }

        /// <summary>
        /// Recuperamos la lista de Aplicaciones o bien podemos filtrar por nombre.
        /// </summary>
        /// <param name="nameApplication">Nombre de la aplicación</param>
        /// <returns>Devuelve un lista de las aplicaciones o la aplicación filtrada por nombre</returns>
        public async Task<List<Applications>> GetApplications(string? nameApplication)
        {
            try
            {
                Logger.LogDebug($"Starting Repository {nameof(GetApplications)}");
                var result = new List<Applications>();
                if (!string.IsNullOrEmpty(nameApplication))
                {
                    result = await DbContext.Applications.Where(o=>o.Name == nameApplication).ToListAsync();
                }
                else
                {
                    result= await DbContext.Applications.OrderBy(o=>o.Id).ToListAsync();
                }
                return result;
            }
            finally
            {
                Logger.LogDebug($"End Repository {nameof(GetApplications)}");
            }
        }  
        /// <summary>
        /// Insertamos una nueva Aplicación
        /// </summary>
        /// <param name="application"></param>
        /// <returns></returns>
        /// <exception cref="ConflictException"></exception>
        public async Task<Applications> AddApplications(Applications application)
        {
            try
            {
                Logger.LogDebug($"Starting Repository {nameof(AddApplications)}");
                Guard.CheckIsNotNull(application, nameof(application));

                var result = await DbContext.Applications.FirstOrDefaultAsync(o=>o.Name == application.Name);
                
                if(result!=null) { throw new ConflictException($"Ya existe una Aplicacion con el nombre [{application.Name}]"); }

                await DbContext.Applications.AddAsync(application).ConfigureAwait(true);
                await DbContext.SaveChangesAsync().ConfigureAwait(true);

                return application;
            }
            finally
            {
                Logger.LogDebug($"End Repository {nameof(AddApplications)}");
            }
        }
        /// <summary>
        /// Borramos la Aplicación por el nombre.
        /// </summary>
        /// <param name="nameApplication"></param>
        /// <returns></returns>
        /// <exception cref="ConflictException"></exception>
        public async Task<bool> DeleteApplications(string nameApplication)
        {
            try
            {
                Logger.LogDebug($"Starting Repository {nameof(DeleteApplications)}");
                Guard.CheckIsNotNullOrWhiteSpace(nameApplication, nameof(nameApplication));

                var result = await DbContext.Applications.FirstOrDefaultAsync(o=>o.Name == nameApplication);
                
                if(result==null) { throw new ConflictException($"No existe una Aplicacion con el nombre [{nameApplication}]"); }

                DbContext.Applications.Remove(result);

                return Convert.ToBoolean(await DbContext.SaveChangesAsync().ConfigureAwait(true));
            }
            finally
            {
                Logger.LogDebug($"End Repository {nameof(DeleteApplications)}");
            }
        }
    }
}

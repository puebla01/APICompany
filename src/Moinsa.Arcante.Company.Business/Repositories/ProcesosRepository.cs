using Moinsa.Arcante.Company.Business.Repositories.Interfaces;
using Moinsa.Arcante.Company.Business.Validations;
using Moinsa.Arcante.Company.Domain.Entities;
using Moinsa.Arcante.Company.Domain.Enums;
using Moinsa.Arcante.Company.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static Moinsa.Arcante.Company.Domain.Enums.EnumList;
using Moinsa.Arcante.Company.Models.Process;

namespace Moinsa.Arcante.Company.Business.Repositories
{
    public class ProcesosRepository:BaseRepository, IProcesosRepository
    {
        public ProcesosRepository(ApiCompanyDbContext dbContext, 
                                    ILogger<ProcesosRepository> logger,
                                    AutoMapper.IMapper mapper): base(dbContext, logger, mapper) 
        {
        }

        public async Task<ProcesosResponseModel> GetProceso(int idOrganizacion)
        {
            try
            {
                Logger.LogDebug($"Starting Repository {GetProceso}");

                ProcesosResponseModel result = await (from pro in DbContext.Procesos
                                join org in DbContext.Organizations on pro.IdOrganizacion equals org.Id
                                select new ProcesosResponseModel { Organizacion=org.Nombre, OrganizacionId=org.Id, Id=pro.Id, estadoProceso=pro.Proceso}).FirstOrDefaultAsync();

                
                return result;
                
            }
            finally
            {
                Logger.LogDebug($"End Repository {GetProceso}");
            }
        }
        public async Task<bool> AddOrUpdateProceso(string connectionString, int idOrg, int estadoProceso,string obs)
        {
            try
            {
                Logger.LogDebug($"Starting {nameof(AddOrUpdateProceso)}");
                var estado = "";
                using (ApiCompanyDbContext context = new ApiCompanyDbContext(connectionString))
                {
                    Procesos proceso = await context.Procesos.FirstOrDefaultAsync(o => o.IdOrganizacion == idOrg);
                    if (proceso == null)
                    {
                        proceso = new Procesos { IdOrganizacion = idOrg, Proceso = Utils.GetDisplayName(estadosProcesosEnum.EnProgreso), Obs = obs };
                        await context.AddRangeAsync(proceso);
                        await context.SaveChangesAsync();
                    }
                    else
                    {
                        switch (estadoProceso)
                        {
                            case (int)estadosProcesosEnum.EnProgreso:
                                estado = Utils.GetDisplayName(estadosProcesosEnum.EnProgreso);
                                break;
                            case (int)estadosProcesosEnum.ProcesoTerminado:
                                estado = Utils.GetDisplayName(estadosProcesosEnum.ProcesoTerminado);
                                break;
                            case (int)estadosProcesosEnum.Conflicto:
                                estado = Utils.GetDisplayName(estadosProcesosEnum.Conflicto);
                                break;
                        }
                        proceso.Proceso = estado;
                        proceso.Obs = obs;
                        await context.SaveChangesAsync();
                    }
                    return true;
                }
            }
            finally
            {
                Logger.LogDebug($"End {nameof(AddOrUpdateProceso)}");
            }

        }
    }
}

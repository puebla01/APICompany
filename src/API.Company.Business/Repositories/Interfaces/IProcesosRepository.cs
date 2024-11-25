using API.Company.Domain.Entities;
using API.Company.Models.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Company.Business.Repositories.Interfaces
{
    public interface IProcesosRepository
    {
        public Task<bool> AddOrUpdateProceso(string connectionString ,int idOrganizacion, int estadoProc, string Obs);

        public Task<ProcesosResponseModel> GetProceso(int idOrg);

    }
}

using Moinsa.Arcante.Company.Domain.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moinsa.Arcante.Company.Models.Process;

namespace Moinsa.Arcante.Company.Infraestructure.Data.Mappings
{
    public class ProcesosProfile:Profile
    {
        public ProcesosProfile()
        {
            CreateMap<ProcesosResponseModel, Procesos>()
                 .ForMember(o => o.IdOrganizacion,
                    p => p.MapFrom(q => q.OrganizacionId))
                  .ForMember(o => o.Proceso,
                    p => p.MapFrom(q => q.estadoProceso))
                  .ReverseMap();
        }
    }
}

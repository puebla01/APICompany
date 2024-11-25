using AutoMapper;
using Moinsa.Arcante.Company.Domain.Entities;
using Moinsa.Arcante.Company.Models.Applications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moinsa.Arcante.Company.Infraestructure.Data.Mappings
{
    public class ApplicationsProfile:Profile
    {
        public ApplicationsProfile()
        {
            CreateMap<Applications, ApplicationsResponseModels>().ReverseMap();
            CreateMap<Applications, ApplicationsRequestModels>().ReverseMap();
        }
    }
}

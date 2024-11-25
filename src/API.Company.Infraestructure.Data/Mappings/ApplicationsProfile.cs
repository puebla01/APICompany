using AutoMapper;
using API.Company.Domain.Entities;
using API.Company.Models.Applications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Company.Infraestructure.Data.Mappings
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

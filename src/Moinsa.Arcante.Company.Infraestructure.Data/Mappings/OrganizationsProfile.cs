using Moinsa.Arcante.Company.Domain.Entities;
using AutoMapper;
using Moinsa.Arcante.Company.Models.Companies;


namespace Moinsa.Arcante.Company.Infraestructure.Data.Mappings
{
    public class OrganizationsProfile:Profile
    {
        public OrganizationsProfile() 
        {
            CreateMap<OrganizationsRequestModel, Organizations>().ReverseMap();
            CreateMap<OrganizationsResponseModel, Organizations>().ReverseMap();
            CreateMap<OrganizationsCreateDDBBRequestModel, Organizations>().ReverseMap();

        }
    }
}

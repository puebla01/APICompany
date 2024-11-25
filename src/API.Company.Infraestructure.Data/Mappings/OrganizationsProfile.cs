using API.Company.Domain.Entities;
using AutoMapper;
using API.Company.Models.Companies;


namespace API.Company.Infraestructure.Data.Mappings
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

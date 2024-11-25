using API.Company.Domain.Entities;
using API.Company.Models.Companies;

namespace API.Company.Business.Repositories.Interfaces
{
    public interface IOrganizationsRepository
    {
        public Task<OrganizationsResponseModel> AddOrganization(Organizations organization);
        public Task<List<OrganizationsResponseModel>> GetOrganization(string? nameOrganization,string? application);
        public Task<Organizations> GetOrganizationByConn(string nameOrganization);
        public Task<bool> DeleteOrganization(int idOrganization);
        public void CreateOrUpdateOrganizacionDDBB(OrganizationsResponseModel org,string sqlPath,string dacpacPath);
        public Task<OrganizationsResponseModel> UpdateOrganization(int idOriginalOrganization ,OrganizationsUpdateRequestModel organization);

    }
}

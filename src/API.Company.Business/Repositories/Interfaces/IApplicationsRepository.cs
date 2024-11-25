using API.Company.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Company.Business.Repositories.Interfaces
{
    public interface IApplicationsRepository
    {
        public Task<List<Applications>> GetApplications(string? nameApplication);
        public Task<Applications> AddApplications(Applications application);
        public Task<bool> DeleteApplications(string nameApplication);
    }
}

using API.Company.Business.Validations;
using API.Company.Infraestructure.Data;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace API.Company.Business.Repositories
{
    public abstract class BaseRepository
    {
        private readonly ApiCompanyDbContext _dbContext;
        private readonly ILogger _logger;
        private IMapper _mapper;


        protected internal ApiCompanyDbContext DbContext => _dbContext;
        protected internal ILogger Logger => _logger;
        protected internal IMapper Mapper => _mapper;
        public BaseRepository(ApiCompanyDbContext dbContext
            , ILogger logger, IMapper mapper
            )
        {
            Guard.CheckIsNotNull(dbContext, nameof(dbContext));
            Guard.CheckIsNotNull(logger, nameof(logger));
            Guard.CheckIsNotNull(mapper, nameof(mapper));
            _dbContext = dbContext;
            _logger = logger;
            _mapper = mapper;
        }

    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace API.Company.Infraestructure.Data
{
    public abstract class BaseDbContext : DbContext
    {
        protected readonly IConfiguration _configuration = null;
        protected readonly ILogger _logger = null;
        public bool AutoUpdateCommonFields { get; set; } = true;

        public BaseDbContext()
        {
        }
        public BaseDbContext(DbContextOptions options) : base(options)
        {
        }

        public BaseDbContext(DbContextOptions options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }
        public BaseDbContext(DbContextOptions options, IConfiguration configuration, ILogger logger) : this(options, configuration)
        {
            _logger = logger;
            _logger?.LogDebug($"{nameof(BaseDbContext)} constructor with logger");
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            _logger?.LogDebug($"{nameof(BaseDbContext)}.{nameof(SaveChangesAsync)} Start");
            UpdateCommonFields();
            var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken).ConfigureAwait(false);
            _logger?.LogDebug($"{nameof(BaseDbContext)}.{nameof(SaveChangesAsync)} End");
            return result;
        }
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            _logger?.LogDebug($"{nameof(BaseDbContext)}.{nameof(SaveChanges)} Start");
            UpdateCommonFields();
            var result = base.SaveChanges(acceptAllChangesOnSuccess);
            _logger?.LogDebug($"{nameof(BaseDbContext)}.{nameof(SaveChanges)} End");
            return result;
        }
        private void UpdateCommonFields()
        {
            _logger?.LogDebug($"{nameof(BaseDbContext)}.{nameof(UpdateCommonFields)} Start");
            if (AutoUpdateCommonFields)
            {
                var changes = ChangeTracker
                                .Entries()
                                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted);

                //Analizamos entidad y accion para llamar a acciones personalizadas
                foreach (var item in changes)
                {
                    switch (item.State)
                    {
                        case EntityState.Deleted:
                            break;

                        case EntityState.Modified:
                            this.UpdateEntityCommonFields(item.Entity);
                            break;

                        case EntityState.Added:
                            this.UpdateEntityCommonFields(item.Entity, true);
                            break;
                    }
                }
            }
            _logger?.LogDebug($"{nameof(BaseDbContext)}.{nameof(UpdateCommonFields)} End");
        }
        private static bool SetPropertyValue(object obj, string propertyName, object value)
        {
            bool result = false;
            try
            {
                PropertyInfo prop = obj.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (null != prop && prop.CanWrite)
                {
                    prop.SetValue(obj, value);
                    result = true;
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }
        private void UpdateEntityCommonFields(object entity, bool adding = false)
        {
            _logger?.LogDebug($"{nameof(BaseDbContext)}.{nameof(UpdateEntityCommonFields)} Start");
           
                if (adding)
                {
                    SetPropertyValue(entity, "fxaltareg", DateTime.Now);
                
                }
            SetPropertyValue(entity, "fxmodreg", DateTime.Now);

            _logger?.LogDebug($"{nameof(BaseDbContext)}.{nameof(UpdateEntityCommonFields)} Start");
        }

    }
}

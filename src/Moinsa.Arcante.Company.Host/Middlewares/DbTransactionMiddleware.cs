using API.Company.Host.Attributes;
using API.Company.Infraestructure.Data;
using Microsoft.AspNetCore.Http.Features;
using Serilog;

namespace API.Company.Host.Middlewares
{
    public class DbTransactionMiddleware
    {
        private readonly RequestDelegate _next;

        public DbTransactionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, ApiCompanyDbContext dbContext)
        {
            try
            {
                Log.Debug($"{nameof(DbTransactionMiddleware)}.{nameof(Invoke)} started");

                // For HTTP GET opening transaction is not required
                if (httpContext.Request.Method.Equals("GET", StringComparison.CurrentCultureIgnoreCase))
                {
                    await _next(httpContext);
                    return;
                }

                // If action is not decorated with TransactionAttribute then skip opening transaction
                var endpoint = httpContext.Features.Get<IEndpointFeature>()?.Endpoint;
                var attribute = endpoint?.Metadata.GetMetadata<TransactionAttribute>();
                if (attribute == null)
                {
                    await _next(httpContext);
                    return;
                }

                InfoTransaction? transaction = null;

                try
                {
                    transaction = await dbContext.BeginTransactionAsync();

                    await _next(httpContext);

                    await dbContext.CommitTransactionAsync(transaction);
                    Log.Debug($"{nameof(DbTransactionMiddleware)}.{nameof(Invoke)} Commited");
                }
                catch (Exception)
                {
                    if (transaction != null)
                    {
                        await dbContext.RollbackTransactionAsync(transaction);
                        Log.Debug($"{nameof(DbTransactionMiddleware)}.{nameof(Invoke)} Rollback done");
                    }

                    throw;
                }
            }
            finally
            {
                Log.Debug($"{nameof(DbTransactionMiddleware)}.{nameof(Invoke)} end");
            }
        }


    }
}

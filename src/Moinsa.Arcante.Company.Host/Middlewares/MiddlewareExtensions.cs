using Microsoft.EntityFrameworkCore;

namespace Moinsa.Arcante.Company.Host.Middlewares
{
    public static class MiddlewareExtensions
    {
        /// <summary>
        /// Adds the <see cref="DbTransactionMiddleware"/> to the specified <see cref="IApplicationBuilder"/> whichs adds Database Transacion capabilities
        /// </summary>
        /// <typeparam name="T">DbContext</typeparam>
        /// <param name="app">IApplicationBuilder</param>
        /// <returns></returns>
        public static IApplicationBuilder UseDbTransaction(this IApplicationBuilder app)
        {
            return app.UseMiddleware<DbTransactionMiddleware>();
        }
    }
}

using API.Company.Business;
using API.Company.Business.Repositories;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Busca los servicios incluidos dentro del ensamblado que implementen el tipo BaseRepository, y los agrega como un servicio Microsoft.Extensions.DependencyInjection.ServiceLifetime.Scoped a la colección si el tipo de servicio aún no se ha registrado.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public static IServiceCollection AddRepositories(this IServiceCollection services, params Assembly[] assemblies)
        {
            Type basetype = typeof(BaseRepository);

            //Registramos la DI para los constructores Lazy<T> de manera generica
            //ver (https://thecodeblogger.com/2021/04/28/delayed-instantiation-using-dependency-injection-in-net/)
            services.AddTransient(typeof(Lazy<>), typeof(LazyInstance<>));

            if (assemblies != null && assemblies.Length != 0)
            {
                TypeInfo[] allTypes = assemblies
                    .Where((Assembly a) => !a.IsDynamic)
                    .Distinct()
                    .SelectMany((Assembly a) => a.DefinedTypes)
                    .ToArray();

                var col = allTypes.Where((TypeInfo t) => t.IsClass && !t.IsAbstract && t.AsType().IsSubclassOf(basetype));
                foreach (var item in col)
                {
                    Type servicio = item.AsType();
                    //Si tiene una Interfaz con el mismo nombre del servicio y el prefijo I,...
                    Type interfaz = item.GetInterfaces().Where(q=> q.Name == $"I{servicio.Name}").FirstOrDefault();
                    if (interfaz != null)
                    {
                        //...registramos la interfaz y el servicio como su implementacion,...
                        services.TryAddScoped(interfaz, servicio);
                    }
                    else
                    {
                        ////...sino lanzamos excepción para mantener la integridad de la arquitectura
                        throw new NullReferenceException($"El interfaz I{servicio.Name} no está definido.");
                    }
                }
            }
            return services;
        }

    }
}

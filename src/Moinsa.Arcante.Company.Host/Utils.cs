using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reflection;

namespace Moinsa.Arcante.Company.Host
{
    public static class Utils
    {
        public static bool isRunningAsService(string[] args)
        {
            bool runningAsService = false;

            if (!Debugger.IsAttached && args?.Where((x) => x.Contains("service"))?.Count() > 0)
            {
                runningAsService = true;
            }
            else
            {
                runningAsService = false;
            }

            return (bool)runningAsService;
        }

        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
           .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
           .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true)
           .AddEnvironmentVariables()
           .Build();

        public static string dbConnectionString()
        {
            return ConfigApp.ConnectionStringArcante?? string.Empty;
        }
        public static string GetRutaArchivoSQLPackage()
        {
            return Configuration["DACPACConnection:RutaArchivoSQLPackage"] ?? string.Empty;
        }
        public static string GetRutaArchivosDACPAC()
        {
            return Configuration["DACPACConnection:RutaArchivosDACPAC"] ?? string.Empty;
        }

        /// <summary>
        /// Metodo para recuperar el nombre de la lista que le pasemos.
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static string GetDisplayName(this Enum enumValue)
        {
            return enumValue.GetType()
              .GetMember(enumValue.ToString())
              .First()
              .GetCustomAttribute<DisplayAttribute>()
              ?.GetName() ?? string.Empty;
        }
    }

}

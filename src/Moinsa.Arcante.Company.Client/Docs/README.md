# Introduction 
Moinsa.Arcante.Company.Client es una libreria que agrupa los clientes http necesarios para llamar al API Rest Moinsa.Arcante.Company.Client.Host

# How to Use
La librería puede ser instanciado pasándole un IConfiguration, o bien, con los parámetros requeridos.
Para la obtención del Token, en el ejemplo siguiente se obtiene manualmente. No obstante, si el token no está establecido y se realizan llamadas a
los controladores que lo requieren se intentará obtener el token internamente con los datos proporcionados en el constructor. De esta manera el manejo del
token podrá ser ajeno a las aplicaciones que utilicen este cliente. 

En la librería se gestionan la encriptación de datos sensibles en el interfaz IConfiguration, siempre y cuando se implemente en el IConfiguration que si se
realizan cambios se guarden automáticamente. La sección de configuración del archivo appSettings es la siguiente:
"ServiceCompanies": {
    "Url": "https://localhost:17117/",
    "User": "?",
    "Password": "?",
    "ApplicationName": "?",
    "DefaultCompany": "?"
  }

```C#
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moinsa.Arcante.Company.Client;
using Moinsa.Arcante.Company.Client.Test;
using Moinsa.Arcante.Company.Models;

namespace ConsoleClientTest
{
    internal class Program
    {
        const string USER_NAME = "1234";
        const string PASSWORD = "1234";
        const string APPLICATION = "ARCANTE";

        static void Main(string[] args)
        {
            var filePathSettings = $@"{AppDomain.CurrentDomain.BaseDirectory}\appsettings.json";
            var builder = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                .AddJsonFile(filePathSettings);

            var _configuration = builder.Build();


            Console.WriteLine("Moinsa.Arcante.Company.Client");
            Console.WriteLine("=====================================================\r\n");

            string url = "https://localhost:17117";

            Console.WriteLine($"BaseUrl = {url}\r\n");
            System.Threading.Thread.Sleep(4000);

            Client client = new Client(_configuration);
            
            Console.WriteLine("Requesting token...");
            var token = client.TokenAuthenticateAsync("", new CredentialsModel
            {
                Username = client.User,
                Password = client.Password
            }).Result;
            client.AccessToken = token;
            Console.WriteLine($"Token = {token}\r\n");


            Console.WriteLine("How to get All companies...");
            var companies = client.OrganizacionesGetAsync(null, APPLICATION, "").GetAwaiter().GetResult();
            Console.WriteLine($"List of Companies = {Newtonsoft.Json.JsonConvert.SerializeObject(companies)}\r\n");

            string connectionString = "Demo";
            string nombreEmpresa = "Demo";
            string nombreAplicacion = "Demo";


            Console.WriteLine("How to add a APPLICATION...");
            try
            {
                var dataCreation = client.AplicacionesPostAsync("", new Moinsa.Arcante.Company.Models.Applications.ApplicationsRequestModels()
                {
                    Name = nombreAplicacion,
                    SourceUpdate = 0
                }).GetAwaiter().GetResult();
                Console.WriteLine($"Aplication created. Data {Newtonsoft.Json.JsonConvert.SerializeObject(dataCreation)}\r\n");
            }
            catch (SwaggerException ex)
            {
                Console.WriteLine($"SwaggerException=>{ex.Message}");
            }

            Console.WriteLine("How to remove a APPLICATION...");
            try
            {
                var dataRemove = client.AplicacionesDeleteAsync(nombreAplicacion, "").GetAwaiter().GetResult;
                Console.WriteLine($"Aplication [{nombreAplicacion}] removed.\r\n");
            }
            catch (SwaggerException ex)
            {
                Console.WriteLine($"SwaggerException=>{ex.Message}");
            }



            Console.WriteLine("How to add a company...");
            try
            {
                var dataCreation = client.OrganizacionesPostAsync("", new Moinsa.Arcante.Company.Models.Companies.OrganizationsRequestModel()
                {
                    CadenaConexion = connectionString,
                    Nombre = nombreEmpresa,
                    Aplicacion =APPLICATION
                }).GetAwaiter().GetResult();
                Console.WriteLine($"Company created. Data {Newtonsoft.Json.JsonConvert.SerializeObject(dataCreation)}\r\n");
            }
            catch (SwaggerException ex)
            {
                Console.WriteLine($"SwaggerException=>{ex.Message}");
            }

            Console.WriteLine("How to remove a company...");
            try
            {
                var dataRemove = client.OrganizacionesDeleteAsync(nombreEmpresa, APPLICATION, "").GetAwaiter().GetResult;
                Console.WriteLine($"Company [{nombreEmpresa}] removed.\r\n");
            }
            catch (SwaggerException ex)
            {
                Console.WriteLine($"SwaggerException=>{ex.Message}");
            }

            

            Console.WriteLine("Press Enter key to end.");
            Console.ReadLine();
        }
    }
}



```
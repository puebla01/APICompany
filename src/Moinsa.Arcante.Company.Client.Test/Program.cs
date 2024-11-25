using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using API.Company.Client;
using API.Company.Models;

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


            Console.WriteLine("API.Company.Client");
            Console.WriteLine("=====================================================\r\n");

            Client client = new Client(_configuration);

            Console.WriteLine($"BaseUrl = {client.BaseUrl}\r\n");

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
                var dataCreation = client.AplicacionesPostAsync("", new API.Company.Models.Applications.ApplicationsRequestModels()
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
                var dataCreation = client.OrganizacionesPostAsync("", new API.Company.Models.Companies.OrganizationsRequestModel()
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


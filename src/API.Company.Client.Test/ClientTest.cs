using FluentAssertions;
using FluentAssertions.Equivalency;
using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;
using API.Company.Models;
using API.Company.Models.Applications;
using API.Company.Models.Companies;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Policy;
using Xunit.Priority;


namespace API.Company.Client.Test
{
    public class ClientTest
    {
        [Fact, Priority(0)]
        public void Authenticate_returns_OK()
        {
            //ARRANGE
            Client client = new Client(Settings.Url ,Settings.User, Settings.Password, Settings.ApplicationName,Settings.DefaultCompany);

            //ACT 
            var token = client.TokenAuthenticateAsync("", new CredentialsModel()
            {
                Username = client.User,
                Password = client.Password                
            }).GetAwaiter().GetResult();

            //ASSERT
            token.Should().NotBeNullOrEmpty();
        }

        [Fact, Priority(0)]
        public void Authenticate_returns_KO()
        {
            //ARRANGE
            Client client = new Client(Settings.Url ,Settings.User, Settings.Password, Settings.ApplicationName,Settings.DefaultCompany);

            //ACT 
            var act = () => client.TokenAuthenticateAsync("", new CredentialsModel()
            {
                Username = string.Empty,
                Password = string.Empty
            }).GetAwaiter().GetResult();
            

            //ASSERT
            act.Should().Throw<SwaggerException>();
        }



        [Fact, Priority(20)]
        public  void GetOrganizacion_returns_OK()
        {
            //ARRANGE
            Client client = new Client(Settings.Url ,Settings.User, Settings.Password, Settings.ApplicationName,Settings.DefaultCompany);

            //ACT
            var act = () => client.OrganizacionesGetAsync("Organizacion_Test", Settings.ApplicationName, "").GetAwaiter().GetResult();

            //ASSERT
            act.Should().NotThrow();
        }

        [Fact, Priority(20)]
        public void GetOrganizacion_returns_KO()
        {
            //ARRANGE
            Client client = new Client(Settings.Url ,Settings.User, Settings.Password, Settings.ApplicationName,Settings.DefaultCompany);

            //ACT
            var result = client.OrganizacionesGetAsync("TEST COMPANY NO EXISTE",Settings.ApplicationName, "").GetAwaiter().GetResult();

            //ASSERT
            result.Count().Should().Be(0);
        }

        [Fact, Priority(20)]
        public void GetAllOrganizaciones_returns_OK()
        {
            //ARRANGE
            Client client = new Client(Settings.Url ,Settings.User, Settings.Password, Settings.ApplicationName,Settings.DefaultCompany);

            //ACT
            var act = () => client.OrganizacionesGetAsync(null, Settings.ApplicationName, "").GetAwaiter().GetResult();

            //ASSER
            act.Should().NotThrow();
        }

        [Fact, Priority(10)]
        public void AddOrganization_returns_OK()
        {            
            //ARRANGE
            Client client = new Client(Settings.Url ,Settings.User, Settings.Password, Settings.ApplicationName,Settings.DefaultCompany);
            OrganizationsRequestModel body = new OrganizationsRequestModel()
            {
                Nombre = "Organizacion_Test",
                CadenaConexion = "Cadenaconexion_Test",
                Aplicacion = Settings.ApplicationName
            };

            //ACT
            var act = () => client.OrganizacionesPostAsync("", body).GetAwaiter().GetResult();

            //ASSERT
            act.Should().NotThrow();
        }

        [Fact, Priority(10)]
        public void AddOrganization_returns_KO()
        {
            //ARRANGE
            Client client = new Client(Settings.Url ,Settings.User, Settings.Password, Settings.ApplicationName,Settings.DefaultCompany);
            OrganizationsRequestModel body = new OrganizationsRequestModel()
            {
                Nombre = "Organizacion_Test55555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555555",
                CadenaConexion = "Cadenaconexion_Test",
                Aplicacion = Settings.ApplicationName
            };


            //ACT
            var act = () => client.OrganizacionesPostAsync("", body).GetAwaiter().GetResult();

            //ASSERT
            act.Should().Throw<SwaggerException>();
        }

        [Fact, Priority(30)]
        public void DeleteOrganization_returns_OK()
        {
            //ARRANGE
            Client client = new Client(Settings.Url ,Settings.User, Settings.Password, Settings.ApplicationName,Settings.DefaultCompany);

            //ACT
            var act = () => client.OrganizacionesDeleteAsync("Organizacion_Test",Settings.ApplicationName, "").GetAwaiter().GetResult();

            //ASSERT
            act.Should().NotThrow();
        }

        [Fact, Priority(30)]
        public void DeleteOrganization_returns_KO()
        {
            //ARRANGE
            Client client = new Client(Settings.Url ,Settings.User, Settings.Password, Settings.ApplicationName,Settings.DefaultCompany);

            //ACT
            var act = () => client.OrganizacionesDeleteAsync("5555555555555555555555555555555555555555",Settings.ApplicationName, "").GetAwaiter().GetResult();

            //ASSERT
            act.Should().Throw<SwaggerException>();
        }

        //-------------------------------------
        [Fact, Priority(10)]
        public void AddApplication_returns_OK()
        {
            //ARRANGE
            Client client = new Client(Settings.Url, Settings.User, Settings.Password, Settings.ApplicationName, Settings.DefaultCompany);
            ApplicationsRequestModels body = new ApplicationsRequestModels()
            {
                Name = $"{Settings.ApplicationName}TEST", 
                SourceUpdate = 0
            };

            //ACT
            var act = () => client.AplicacionesPostAsync("", body).GetAwaiter().GetResult();

            //ASSERT
            act.Should().NotThrow();
        }

        [Fact, Priority(10)]
        public void AddApplication_returns_KO()
        {
            //ARRANGE
            Client client = new Client(Settings.Url, Settings.User, Settings.Password, Settings.ApplicationName, Settings.DefaultCompany);
            ApplicationsRequestModels body = new ApplicationsRequestModels()
            {
                Name = "",
                SourceUpdate = 0
            };


            //ACT
            var act = () => client.AplicacionesPostAsync("", body).GetAwaiter().GetResult();

            //ASSERT
            act.Should().Throw<SwaggerException>();
        }

        [Fact, Priority(30)]
        public void DeleteApplication_returns_OK()
        {
            //ARRANGE
            Client client = new Client(Settings.Url, Settings.User, Settings.Password, Settings.ApplicationName, Settings.DefaultCompany);

            //ACT
            var act = () => client.AplicacionesDeleteAsync($"{Settings.ApplicationName}TEST","").GetAwaiter().GetResult();

            //ASSERT
            act.Should().NotThrow();
        }

        [Fact, Priority(30)]
        public void DeleteApplication_returns_KO()
        {
            //ARRANGE
            Client client = new Client(Settings.Url, Settings.User, Settings.Password, Settings.ApplicationName, Settings.DefaultCompany);

            //ACT
            var act = () => client.AplicacionesDeleteAsync("5555555555555555555555555555555555555555", "").GetAwaiter().GetResult();

            //ASSERT
            act.Should().Throw<SwaggerException>();
        }
    }
}

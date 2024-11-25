using API.Company.Controllers;
using API.Company.Business.Repositories.Interfaces;
using API.Company.Host.Resources;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Annotations;
using API.Company.Business.Validations;
using System.Diagnostics;
using Serilog;
using API.Company.Domain.Enums;
using API.Company.Domain.Entities;
using System.Diagnostics.Metrics;
using API.Company.Models.Companies;
using System.Collections.Immutable;
using API.Company.Business;

namespace API.Company.Host.Controllers
{
    /// <summary>
    /// Endpoints relacionados Organizaciones
    /// </summary>
    [SwaggerTag("Crear, obtener, actualizar y borrar Organizaciones")]
    public class OrganizacionesController :  BaseControllerV1<OrganizacionesController, ApiCompanyResource>
    {
        private readonly IOrganizationsRepository _organizationsRepository;
        private readonly IApplicationsRepository _applicationsRepository;
        private readonly IMapper _mapper;
        private readonly IProcesosRepository _procesosRepository;

        /// <summary>
        /// Constructor controlador Organizaciones
        /// </summary>
        /// <param name="organizationsRepository"></param>
        /// <param name="logger"></param>
        /// <param name="localizer"></param>
        /// <param name="mapper"></param>
        public OrganizacionesController (
                    IOrganizationsRepository organizationsRepository,
                    IApplicationsRepository applicationsRepository,
                    IProcesosRepository procesosRepository,
                    ILogger<OrganizacionesController> logger,
                    IStringLocalizer<ApiCompanyResource> localizer,
                    IMapper mapper):base(logger,localizer)
        {
            _organizationsRepository = organizationsRepository;
            _applicationsRepository = applicationsRepository;   
            _mapper = mapper;
            _procesosRepository = procesosRepository;
        }

        /// <summary>
        /// Recuperamos los datos de la organización filtrando por su nombre.
        /// </summary>
        /// <param name="NombreOrganizacion">Nombre de la organización por el que vamos a realizar la búsqueda</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<OrganizationsResponseModel>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status500InternalServerError)]
        public virtual async Task<IActionResult>GetOrganization(string? NombreOrganizacion, string? NombreAplicacion)
        {
            try
            {
                _logger.LogDebug($"Start {nameof(GetOrganization)}");
                var result = await _organizationsRepository.GetOrganization(NombreOrganizacion,NombreAplicacion);

                if (result == null)
                {
                    return NotFound($"La Organizacion con nombre [{NombreOrganizacion}] para la Aplicación [{NombreAplicacion}] que busca no existe");
                }
                _logger.LogDebug($"End {nameof(GetOrganization)}");

                return Ok(result);
            }
            finally
            {
                _logger.LogDebug($"Start {nameof(GetOrganization)}");
            }
        }


        /// <summary>
        /// Insertamos una nueva organización 
        /// </summary>
        /// <param name="organization"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(OrganizationsResponseModel), StatusCodes.Status201Created)]
        public async Task<IActionResult> AddOrganization([FromBody] OrganizationsRequestModel organization)
        {
            try
            {
                _logger.LogDebug($"Start {nameof(AddOrganization)}");
                Guard.CheckIsNotNullOrWhiteSpace(organization.Nombre, nameof(organization.Nombre));
                Guard.CheckIsNotNullOrWhiteSpace(organization.CadenaConexion, nameof(organization.CadenaConexion));
                Guard.CheckIsNotNullOrWhiteSpace(organization.Aplicacion, nameof(organization.Aplicacion));

                var app = await _applicationsRepository.GetApplications(organization.Aplicacion).ConfigureAwait(false);
                if(!app.Any()) { return NotFound($"La Aplicación con nombre [{organization.Aplicacion}] no existe"); }

                var org = await _organizationsRepository.GetOrganization(organization.Nombre, organization.Aplicacion).ConfigureAwait(false);

                if (org.Any())
                {
                    return Conflict($"La Organizacion que busca ya existe [{organization.Nombre}]");
                }
                else
                {
                    var orgByName = await _organizationsRepository.GetOrganizationByConn(organization.CadenaConexion.ToString()).ConfigureAwait(false);
                    if (orgByName != null)
                    {
                        return Conflict($"La cadena de conexión que intenta usar, ya existe para la Organizacion con nombre [{orgByName.Nombre}] y su estado es [{orgByName.Estado}]");
                    }
                }
                var newOrganization = _mapper.Map<Organizations>(organization);
                newOrganization.IdApplication = app.First().Id;
                var result = await _organizationsRepository.AddOrganization(newOrganization).ConfigureAwait(true);

                return Created(new Uri($"{Request.Path}?Organizacion creada", UriKind.Relative), result);
                
            }
            finally     
            {
                _logger.LogDebug($"End {nameof(AddOrganization)}");

            }
        }

        /// <summary>
        /// Borramos los datos de la organización por su nombre y aplicacion
        /// </summary>
        /// <param name="nombreOrganizacion"></param>
        /// <param name="nombreAplicacion"></param>
        /// <returns></returns>
        [HttpDelete]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType( StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteOrganization([FromQuery] string nombreOrganizacion, string? nombreAplicacion)
        {
            try
            {

                _logger.LogDebug($"Start {nameof(AddOrganization)}");
                Guard.CheckIsNotNullOrWhiteSpace(nombreOrganizacion, nameof(nombreOrganizacion));

                var org = await _organizationsRepository.GetOrganization(nombreOrganizacion,nombreAplicacion).ConfigureAwait(false);

                if (!org.Any())
                {
                    return Conflict($"La Organizacion con nombre: [{nombreOrganizacion}] no existe.");
                }
                else if (org.Count() > 1) 
                {
                    return Conflict($"Existe más de una Organizacion con nombre: [{nombreOrganizacion}] y aplicación {nombreAplicacion}.");
                }

                var result = await _organizationsRepository.DeleteOrganization(org[0].Id).ConfigureAwait(true);

                if (result)
                {
                    return Ok();
                }
                else
                {
                    return Conflict($"No ha sido posible borrar la Organización con nombre [{nombreOrganizacion}]");
                }
            }
            finally
            {

                _logger.LogDebug($"End {nameof(AddOrganization)}");
            }
        }


        /// <summary>
        /// Actualizamos los datos de la organización por su nombre y aplicacion
        /// </summary>
        /// <param name="nombreOrganizacion"></param>
        /// <param name="nombreAplicacion"></param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateOrganization([FromQuery] string nombreOrganizacion, [FromQuery] string? nombreAplicacion, [FromBody]OrganizationsUpdateRequestModel organizacion)
        {
            try
            {

                _logger.LogDebug($"Start {nameof(AddOrganization)}");
                Guard.CheckIsNotNullOrWhiteSpace(nombreOrganizacion, nameof(nombreOrganizacion));

                var org = await _organizationsRepository.GetOrganization(nombreOrganizacion, nombreAplicacion).ConfigureAwait(false);

                if (!org.Any())
                {
                    return Conflict($"La Organizacion con nombre: [{nombreOrganizacion}] para la aplicación [{nombreAplicacion}] no existe.");
                }
                else if (org.Count() > 1)
                {
                    return Conflict($"Existe más de una Organizacion con nombre: [{nombreOrganizacion}] y aplicación {nombreAplicacion}.");
                }

                var result = await _organizationsRepository.UpdateOrganization(org[0].Id,organizacion).ConfigureAwait(true);
                if (result!=null)
                {
                    return Ok(result);
                }
                else
                {
                    return Conflict($"No ha sido posible borrar la Organización con nombre [{nombreOrganizacion}]");
                }
            }
            finally
            {

                _logger.LogDebug($"End {nameof(AddOrganization)}");
            }
        }


        /// <summary>
        /// Insertamos una nueva organización y si no existe la base de datos se crea siguiendo esa cadena de conexión.
        /// El nombre es con el que se guarda la organización y no con el que se crea la bases de datos que se escoge de la cadena.
        /// </summary>
        /// <param name="organization"></param>
        /// <returns></returns>
        [HttpPost("CreateOrganizacionDDBB")]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(OrganizationsResponseModel), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateOrganizacionDDBB([FromBody] OrganizationsCreateDDBBRequestModel organization)
        {
            try
            {
                _logger.LogDebug($"Start {nameof(CreateOrganizacionDDBB)}");
                Guard.CheckIsNotNullOrWhiteSpace(organization.Nombre, nameof(organization.Nombre));
                Guard.CheckIsNotNullOrWhiteSpace(organization.CadenaConexion, nameof(organization.CadenaConexion));
                Guard.CheckIsNotNullOrWhiteSpace(organization.Aplicacion, nameof(organization.Aplicacion));

                var org = await _organizationsRepository.GetOrganization(organization.Nombre, organization.Aplicacion).ConfigureAwait(false);

                if (org.Any())
                {
                    return Conflict($"Ya existe una organizacion con el nombre [{organization.Nombre}] para la Aplicación[{organization.Aplicacion}]. Revise si quiere actualizar el nombre y la cadena.");
                }
                else
                {
                    var orgByName= await _organizationsRepository.GetOrganizationByConn(organization.CadenaConexion.ToString()).ConfigureAwait(false);
                    if (orgByName != null)
                    {
                        return Conflict($"La cadena de conexión que intenta usar, ya existe para la Organizacion con nombre [{orgByName.Nombre}] para la Aplicación con id [{orgByName.IdApplication}] y está siendo usada");
                    }
                }
                var app = await _applicationsRepository.GetApplications(organization.Aplicacion).ConfigureAwait(false);
                if (!app.Any()) { return NotFound($"No existe una Aplicación con nombre [{organization.Aplicacion}]"); }
                
                var organizacionMap = _mapper.Map<Organizations>(organization);
                organizacionMap.Estado = Utils.GetDisplayName(EnumList.listEstadosBBDD.Actualizado);
                organizacionMap.IdApplication = app.First().Id;

                var newOrganizacion = await _organizationsRepository.AddOrganization(organizacionMap).ConfigureAwait(true);

                _organizationsRepository.CreateOrUpdateOrganizacionDDBB(newOrganizacion, Utils.GetRutaArchivoSQLPackage(), Utils.GetRutaArchivosDACPAC());
                return Created(new Uri($"{Request.Path}?Organizacion creada", UriKind.Relative), await _organizationsRepository.GetOrganization(organization.Nombre, organization.Aplicacion).ConfigureAwait(true));
            }
            finally
            {
                _logger.LogDebug($"End {nameof(CreateOrganizacionDDBB)}");
            }
        }
        /// <summary>
        /// Metodo para actualizar la base de datos.
        /// </summary>
        /// <param name="organization"></param>
        /// <returns></returns>
        [HttpPut("UpdateOrganizacionDDBB")]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(OrganizationsResponseModel), StatusCodes.Status201Created)]
        public async Task<IActionResult> UpdateOrganizacionDDBB([FromBody] OrganizationsCreateDDBBRequestModel organization)
        {
            try
            {
                _logger.LogDebug($"Start {nameof(CreateOrganizacionDDBB)}");
                Guard.CheckIsNotNullOrWhiteSpace(organization.Nombre, nameof(organization.Nombre));
                Guard.CheckIsNotNullOrWhiteSpace(organization.CadenaConexion, nameof(organization.CadenaConexion));

                var org = await _organizationsRepository.GetOrganization(organization.Nombre, organization.Aplicacion).ConfigureAwait(false);
                if (!org.Any())
                {
                    return Conflict($"No existe una organizacion con el nombre [{organization.Nombre}].");
                }
                //Si la organizacion tiene entorno y es distinta de la que quiero actualziar lazanamos error
                var orgToUpdate=org.First();
                if (!string.IsNullOrEmpty(orgToUpdate.Entorno) && orgToUpdate.Entorno != organization.Entorno)
                {
                    return Conflict($"Se está intentando actualizar del entorno [{orgToUpdate.Entorno}] al entorno [{organization.Entorno}]");
                }
                orgToUpdate.Estado = Utils.GetDisplayName(EnumList.listEstadosBBDD.Actualizado);
                orgToUpdate.Entorno = organization.Entorno;


                _organizationsRepository.CreateOrUpdateOrganizacionDDBB(_mapper.Map<OrganizationsResponseModel>(orgToUpdate), Utils.GetRutaArchivoSQLPackage(), Utils.GetRutaArchivosDACPAC());
                return Ok("Iniciando Proceso");
            }
            finally
            {
                _logger.LogDebug($"End {nameof(CreateOrganizacionDDBB)}");

            }
        }

    }
}

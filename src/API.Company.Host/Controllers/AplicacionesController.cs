using API.Company.Controllers;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using API.Company.Business.Repositories.Interfaces;
using API.Company.Domain.Entities;
using API.Company.Domain.Enums;
using API.Company.Host.Resources;
using API.Company.Models.Applications;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Diagnostics;

namespace API.Company.Host.Controllers
{
    /// <summary>
    /// Endpoints relacionados con las Aplicaciones
    /// </summary>
    public class AplicacionesController :BaseControllerV1<AplicacionesController,ApiCompanyResource>
    {
        private readonly IMapper _mapper;
        private readonly IApplicationsRepository _applicationsRepository;
        public AplicacionesController(
                ILogger<AplicacionesController> logger,
                IStringLocalizer<ApiCompanyResource> localizer,
                IApplicationsRepository applicationsRepository,
                IMapper mapper) :base (logger,localizer)
        {
            _mapper = mapper;
            _applicationsRepository = applicationsRepository;
        }
        /// <summary>
        /// Recupera las organizaciones 
        /// </summary>
        /// <param name="NombreAplicacion"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<ApplicationsResponseModels>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public virtual async Task<IActionResult> GetAplicaciones(string? NombreAplicacion)
        {
            try
            {
                _logger.LogDebug($"Starting {nameof(GetAplicaciones)}");
                var result = await _applicationsRepository.GetApplications(NombreAplicacion).ConfigureAwait(false);

                if(result == null)
                {
                    return NotFound($"No exite ninguna Aplicación con el nombre [{NombreAplicacion}]");
                }
                else
                {
                    return Ok(_mapper.Map<List<ApplicationsResponseModels>>(result));  
                }
            }
            finally 
            { 
                _logger.LogDebug($"End {nameof(GetAplicaciones)}");
            }
        }
        /// <summary>
        /// Inserta una Aplicacion
        /// </summary>
        /// <param name="application"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApplicationsResponseModels), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public virtual async Task<IActionResult> AddAplicaciones(ApplicationsRequestModels application)
        {
            try
            {
                _logger.LogDebug($"Starting {nameof(GetAplicaciones)}");
                var result = await _applicationsRepository.GetApplications(application.Name).ConfigureAwait(false);

                if(result.Any())
                {
                    return Conflict($"Ya exite una Aplicación con el nombre [{application.Name}]");
                }

                var newApplication = await _applicationsRepository.AddApplications(_mapper.Map<Applications>(application));
                return Created(new Uri($"{Request.Path}?Aplicacion creada", UriKind.Relative), _mapper.Map<ApplicationsResponseModels>(newApplication));
               
            }
            finally 
            { 
                _logger.LogDebug($"End {nameof(GetAplicaciones)}");
            }
        }
        /// <summary>
        /// Borra una aplicacion filtrando por nombre 
        /// </summary>
        /// <param name="nombreAplicacion"></param>
        /// <returns></returns>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public virtual async Task<IActionResult> DeleteAplicaciones(string nombreAplicacion)
        {
            try
            {
                _logger.LogDebug($"Starting {nameof(GetAplicaciones)}");
                var result = await _applicationsRepository.GetApplications(nombreAplicacion).ConfigureAwait(false);

                if(!result.Any())
                {
                    return NotFound($"No exite ninguna Aplicación con el nombre [{nombreAplicacion}]");
                }

                var delete = await _applicationsRepository.DeleteApplications(nombreAplicacion);
                if (delete)
                {
                    return Ok();  
                }
                else
                {
                    return Conflict($"No ha sido posible eliminar la Aplicación con nombre [{nombreAplicacion}] de la base de datos.");
                }
               
            }
            finally 
            { 
                _logger.LogDebug($"End {nameof(GetAplicaciones)}");
            }
        }
    }
}

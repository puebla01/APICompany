using Moinsa.Arcante.Company.Business.Repositories.Interfaces;
using Api.Company.Controllers;
using Moinsa.Arcante.Company.Host.Resources;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Annotations;
using Moinsa.Arcante.Company.Models.Process;

namespace Moinsa.Arcante.Company.Host.Controllers
{
    /// <summary>
    /// Endpoints relacionados con los Procesos de las organizaicones
    /// </summary>
    [SwaggerTag("Crear, obtener, actualizar y borrar Procesos")]
    public class ProcesosController : BaseControllerV1<ProcesosController,ApiCompanyResource>
    {
        private readonly IProcesosRepository _procesosRepository;
        private readonly IMapper _mapper;
        
        public ProcesosController (
                IProcesosRepository procesosRepository, 
                ILogger<ProcesosController> logger,
                IStringLocalizer<ApiCompanyResource> localizer,
                IMapper mapper) : base(logger, localizer)
        {
            _procesosRepository = procesosRepository;
        }

        /// <summary>
        /// Recuperamos el Proceso en el que se encuentra organizacion filtrando por id.
        /// </summary>
        /// <param name="NombreOrganizacion">Nombre de la organización por el que vamos a realizar la búsqueda</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(ProcesosResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Route(nameof(GetProceso))]
        public virtual async Task<IActionResult> GetProceso(int idOrganizacion)
        {
            try
            {
                _logger.LogDebug($"Starting {nameof(GetProceso)}");
                var proceso= await _procesosRepository.GetProceso(idOrganizacion).ConfigureAwait(false);
                if (proceso == null)
                {
                    return NotFound($"No existe un proceso para la Organización con id {idOrganizacion}");
                }
                return Ok(proceso);
            }
            finally
            {
                _logger.LogDebug($"End {nameof(GetProceso)}");
            }
        }
    }
}

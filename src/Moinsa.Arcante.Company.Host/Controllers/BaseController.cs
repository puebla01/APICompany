using Moinsa.Arcante.Company.Host.Attributes;
using Moinsa.Arcante.Company.Host.Enums;
using Moinsa.Arcante.Company.Host.Filters;
using IO.Swagger.Attributes;
using IO.Swagger.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Company.Controllers
{
    /// <summary>
    /// Controlador base para añadir atributos comunes a todos los Controladores
    /// </summary>
    [ApiController]
    [Authorize(AuthenticationSchemes = BearerAuthenticationHandler.SchemeName)]
    [Transaction]
    //[Route("/api/v{version:apiVersion}/[controller]")]
    [Route("[controller]")]
    [ValidateModelState]
    [SwaggerResponse(statusCode: StatusCodes.Status400BadRequest, type: typeof(string), description: StatusCodesDescriptions.Status400BadRequest)]
    [SwaggerResponse(statusCode: StatusCodes.Status401Unauthorized, type: typeof(string), description: StatusCodesDescriptions.Status401Unauthorized)]
    [SwaggerResponse(statusCode: StatusCodes.Status409Conflict, type: typeof(string), description: StatusCodesDescriptions.Status409Conflict)]
    [SwaggerResponse(statusCode: StatusCodes.Status500InternalServerError, type: typeof(ProblemDetails), description: StatusCodesDescriptions.Status500InternalServerError)]
    [Consumes("application/json")]
    [AcceptedLanguageHeader(false)]
    public class BaseController<T1,T2> : ControllerBase where T1: class where T2 : class
    {
        /// <summary>
        /// Servicio de logger
        /// </summary>
        protected readonly ILogger<T1> _logger;
        /// <summary>
        /// Servicio de localizador
        /// </summary>
        protected readonly IStringLocalizer<T2> _localizer;

        private int? _userId = null;

        /// <summary>
        /// Get id user Arcante (codope) from then token used to authenticate
        /// </summary>
        protected int UserIdFromClaims
        {
            get
            { 
                if (_userId == null)
                {
                    var claim = HttpContext.User.Claims.FirstOrDefault(q => q.Type == "codope")?.Value;
                    if (string.IsNullOrEmpty(claim))
                    {
                        _userId = 0;
                    }
                    else
                    {
                        int aux = 0;
                        if (int.TryParse(claim, out aux))
                            _userId = aux;
                    }
                }
                return _userId ?? 0;
            }
        }

        /// <summary>
        /// Constructores genéricos del controlador con log y localizer
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="localizer"></param>
        public BaseController(ILogger<T1> logger, IStringLocalizer<T2> localizer)
        {
            _logger = logger;
            _localizer = localizer;
        }
    }

    /// <summary>
    /// Controlador base para version 1
    /// </summary>
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "V1")]
    public class BaseControllerV1<T1,T2>:  BaseController<T1,T2> where T1: class where T2: class
    {
        /// <summary>
        /// Constructores genéricos del controlador V1 con log y localizer
        /// </summary>
        /// <param name="logger">Servicio de logger</param>
        /// <param name="localizer">Servicio de localizador</param>
        public BaseControllerV1(ILogger<T1> logger,
            IStringLocalizer<T2> localizer) : base(logger, localizer)
        { 
        }
    }
}
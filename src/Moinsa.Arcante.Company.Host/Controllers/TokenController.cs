
using API.Company.Business.Validations;
using API.Company.Host;
using API.Company.Host.Enums;
using API.Company.Host.Filters;
using API.Company.Host.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Company.Models;

namespace API.Company.Controllers
{
    /// <summary>
    /// Enpoints relacionados con el Token
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "V1")]
    [Route("[controller]")]
    [SwaggerTag("Obtención de token")]
    [AcceptedLanguageHeader(false)]
    public class TokenController : ControllerBase
    {
        public IConfiguration Configuration;
        private readonly ILogger<TokenController> Logger;
        private readonly IStringLocalizer<ApiCompanyResource> Localizer;
        private readonly string DefaultLanguage = "es-ES";

        /// <summary>
        /// Constructor controlador token
        /// </summary>
        /// <param name="config"></param>
        /// <param name="logger"></param>
        /// <param name="localizer"></param>
        public TokenController(IConfiguration config,
                               ILogger<TokenController> logger,
                               IStringLocalizer<ApiCompanyResource> localizer)
        {
            ArgumentNullException.ThrowIfNull(config, nameof(config));
            ArgumentNullException.ThrowIfNull(logger, nameof(logger));

            Configuration = config;
            Logger = logger;
            Localizer = localizer;
        }

        /// <summary>
        /// Obtiene un Jwt token para ser utilizado en posteriores llamados al web api.
        /// Son requeridos usuario y contraseña.
        /// </summary>
        /// <param name="credentialsModel"></param>
        [HttpPost]
        [AllowAnonymous]
        [SwaggerResponse(statusCode: StatusCodes.Status200OK, type: typeof(string), contentTypes: "text/plain")]
        [SwaggerResponse(statusCode: StatusCodes.Status400BadRequest, type: typeof(string), description: StatusCodesDescriptions.Status400BadRequest)]
        [SwaggerResponse(statusCode: StatusCodes.Status500InternalServerError, type: typeof(ProblemDetails), description: StatusCodesDescriptions.Status500InternalServerError)]
        [Route(nameof(Authenticate))]
        public async Task<IActionResult> Authenticate([FromBody] CredentialsModel credentialsModel)
        {
            Logger.LogDebug($"Executing {nameof(Authenticate)}");
            if (!string.IsNullOrEmpty(credentialsModel.Username) && !string.IsNullOrEmpty(credentialsModel.Password))
            {
                if (IsValidUser(credentialsModel.Username, credentialsModel.Password, out string culture))
                {
                    string jwtSubject = Configuration["Jwt:Subject"] ?? "";

                    var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, jwtSubject),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        // new Claim(JwtRegisteredCustomClaimNames.Codope, userid.ToString()),
                        //new Claim(JwtRegisteredCustomClaimNames.Nomope, response.User.Name),
                        //new Claim(JwtRegisteredCustomClaimNames.Apeope, response.User.LastName),
                        //new Claim(JwtRegisteredCustomClaimNames.Valnomope, response.User.UserName),
                        // new Claim(JwtRegisteredCustomClaimNames.Codalm, codalm.ToString() ?? "0"),
                        new Claim(JwtRegisteredCustomClaimNames.Culture, culture)
                    };

                    string jwtKey = Configuration["Jwt:Key"] ?? "";
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    //Aqui establecemos la caducidad del Token
                    var expirationDate = DateTime.UtcNow.AddDays(1);

                    var token = new JwtSecurityToken(
                        Configuration["Jwt:Issuer"],
                        Configuration["Jwt:Audience"],
                        claims,
                        expires: expirationDate,
                        signingCredentials: signIn
                    );
                    
                    Logger.LogDebug("Exiting Authenticate");
                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }
                else
                {
                    return BadRequest(
                        HelperResources.GetString(Localizer,
                        nameof(ApiCompanyResource.TokenController_CredencialesIncorrectas)));
                }
            }
            else
            {
                return BadRequest(
                    HelperResources.GetString(Localizer, 
                    nameof(ApiCompanyResource.TokenController_CredencialesIncorrectas)));
            }
        }

        private bool IsValidUser(string username, string password, out string userLanguage)
        {
               
            var result= false;
            userLanguage = DefaultLanguage;

            result = (username == ConfigApp.User && password == ConfigApp.Password);

            return result;
        }
    }
}
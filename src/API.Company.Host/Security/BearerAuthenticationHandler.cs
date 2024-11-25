using API.Company.Host.Enums;
using API.Company.Infraestructure.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace IO.Swagger.Security
{
    /// <summary>
    /// class to handle bearer authentication.
    /// </summary>
    public class BearerAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        /// <summary>
        /// scheme name for authentication handler.
        /// </summary>
        public const string SchemeName = "Bearer";

        private readonly ApiCompanyDbContext DbContext;
        private readonly IConfiguration Configuration;

        public BearerAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
                                           ILoggerFactory logger,
                                           UrlEncoder encoder,
                                           ISystemClock clock,
                                           IConfiguration configuration,
                                           ApiCompanyDbContext dbContext) : base(options, logger, encoder, clock)
        {
            this.Configuration = configuration;
            this.DbContext = dbContext;
        }

        /// <summary>
        /// verify that require authorization header exists.
        /// </summary>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {
                string message = "";
                this.Logger.LogDebug($"{nameof(HandleAuthenticateAsync)} START");

                IEnumerable<Claim>? claims = null;
                var endpoint = Context.GetEndpoint();
                if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
                {
                    message = "Anonymous Endpoint";
                    this.Logger.LogDebug($"{nameof(HandleAuthenticateAsync)}: {message}");
                    return AuthenticateResult.NoResult();
                }

                if (!Request.Headers.ContainsKey("Authorization"))
                {
                    message = "Missing Authorization Header";
                    this.Logger.LogDebug($"{nameof(HandleAuthenticateAsync)}: {message}");
                    return AuthenticateResult.Fail(message);
                }
                try
                {
                    var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                    if (authHeader != null && authHeader.Parameter != null)
                    {
                        string key = Configuration["Jwt:Key"]?.ToString() ?? "";
                        string token = authHeader.Parameter;
                        var handler = new JwtSecurityTokenHandler();
                        handler.ValidateToken(token, new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(key)
                            ),
                            ValidateIssuer = false,
                            ValidateAudience = false,
                            // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                            ClockSkew = TimeSpan.Zero
                        }, out SecurityToken validatedToken);

                        var jwtToken = (JwtSecurityToken)validatedToken;

                        claims = jwtToken.Claims;
                        var cultureInfo = claims.First(claim => claim.Type == JwtRegisteredCustomClaimNames.Culture).Value;
                        //TODO: [ALV] 30012024 Usuario se va a insertar?

                        //var codope = int.Parse(
                        //    claims.First(claim => claim.Type == JwtRegisteredCustomClaimNames.Codope).Value
                        //);

                        //SetCurrentCulture(!string.IsNullOrEmpty(Request.Headers.AcceptLanguage)?Request.Headers.AcceptLanguage:cultureInfo);

                        //Sobreescribimos el cod usuario de la cadena de conexion, por el usuario extraido del token
                       //DbContext.currentUserCodOpe = codope;
                    }
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex.Message);
                    message = "Invalid Authorization Header";
                    this.Logger.LogDebug($"{nameof(HandleAuthenticateAsync)}:  {message}");
                    return AuthenticateResult.Fail("Invalid Authorization Header");
                }

                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                message = "Success";
                this.Logger.LogDebug($"{nameof(HandleAuthenticateAsync)}:  {message}");

                return AuthenticateResult.Success(ticket);
            }
            finally
            {
                this.Logger.LogDebug($"{nameof(HandleAuthenticateAsync)} END");
            }
        }

        //private void SetCurrentCulture(string cultureInfo)
        //{
        //    try
        //    {
        //        Thread.CurrentThread.CurrentCulture = new CultureInfo(cultureInfo);
        //        Thread.CurrentThread.CurrentUICulture = new CultureInfo(cultureInfo);
        //    }            
        //    catch(Exception ex)
        //    {
        //        this.Logger.LogError(ex, $"Error {nameof(SetCurrentCulture)}");
        //    }
        //}
    }
}
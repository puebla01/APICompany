using Moinsa.Arcante.Company.Business.Repositories;
using Moinsa.Arcante.Company.Host;
using Moinsa.Arcante.Company.Host.Controllers;
using Moinsa.Arcante.Company.Host.Filters;
using Moinsa.Arcante.Company.Host.Middlewares;
using Moinsa.Arcante.Company.Infraestructure.Data;
using CliWrap;
using IO.Swagger.Filters;
using IO.Swagger.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.Filters;
using System.Globalization;
using System.Net;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// AddAsync services to the container.

#region Initializing Services

builder.Host.UseSerilog((context, configuration) =>
{
    configuration
    .ReadFrom.Configuration(context.Configuration);
});

Log.Information($"Starting {nameof(Api)}");

if (!MustBeStarted().Result)
{
    Console.WriteLine("The service must not be started because commandline indicate action installing/uninstalling");
}

UpdateMigration();


builder.Services.AddCors();

//[FCP] establecemos las urls en minusculas como buena practica
builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

builder.Services.AddMemoryCache();

builder.Services.AddHttpLogging(logging =>
{
    // Customize HTTP logging here.
    logging.LoggingFields = HttpLoggingFields.All;
    //logging.RequestHeaders.Add("My-Request-Header");
    //logging.ResponseHeaders.Add("My-Response-Header");
    //logging.MediaTypeOptions.AddText("application/javascript");
    logging.RequestBodyLogLimit = 4096;
    logging.ResponseBodyLogLimit = 4096;
});

builder.Services.AddDbContext<ApiCompanyDbContext>(options =>
    options.UseSqlServer(Utils.dbConnectionString()
    )
#if DEBUG
    .EnableSensitiveDataLogging()
#endif
);

builder.Services.AddLocalization(
    options =>
    {
        options.ResourcesPath = "Resources";

    }
);

//Repositories
builder.Services.AddRepositories(
    typeof(BaseRepository).Assembly
);

//Mappings local
builder.Services.AddAutoMapper(
    typeof(ApiCompanyDbContext).Assembly
);


// AddAsync services to the container.
builder.Services.AddControllers(options =>
{
    options.InputFormatters.Insert(0, MyJPIF.GetJsonPatchInputFormatter());
});

builder.Services.AddProblemDetails(o => o.CustomizeProblemDetails = ctx =>
{
    //Modificamos el traceId para que coincida con el TraceIdentifier de la peticion Http
    ctx.ProblemDetails.Extensions["traceId"] = ctx.HttpContext.TraceIdentifier;
    ctx.ProblemDetails.Extensions.Add("SupportMessage", $"Provide the TraceId '{ctx.HttpContext.TraceIdentifier}' to the support team for further analysis.");

    //En Development exception sera nulo ya que esta interfiriendo el uso de UseDeveloperExceptionPage
    var exception = ctx.HttpContext.Features.Get<IExceptionHandlerPathFeature>()?.Error;
    if (exception != null)
    {
        ctx.ProblemDetails.Title = exception.GetType().Name;
    }

    //Obtenemos el tipo de excepcion
    switch (exception)
    {
        case SqlException ex:
            ctx.ProblemDetails.Status = (int)HttpStatusCode.InternalServerError;
            ManageSqlException(ex, ctx.ProblemDetails);
            break;

        case DbUpdateConcurrencyException e:
            ctx.ProblemDetails.Status = (int)HttpStatusCode.Conflict;
            ctx.ProblemDetails.Detail = e.Message;
            break;

        case DbUpdateException e:
            var messageDb = e.InnerException?.Message;
            if (messageDb != null)
            {
                var stringToFind = "Mensaje:";
                var indexOfMensaje = messageDb.IndexOf(stringToFind);
                if (indexOfMensaje >= 0)
                {
                    messageDb = messageDb.Substring(indexOfMensaje + stringToFind.Length);
                    var indexOfLf = messageDb.IndexOf("\r");
                    if (indexOfLf >= 0)
                    {
                        messageDb = messageDb.Substring(0, indexOfLf);
                    }
                }
            }
            ctx.ProblemDetails.Status = (int)HttpStatusCode.Conflict;
            ctx.ProblemDetails.Detail = messageDb;
            break;

        case KeyNotFoundException e:
            ctx.ProblemDetails.Status = (int)HttpStatusCode.Conflict;
            ctx.ProblemDetails.Detail = e.Message;
            break;


        case Exception ex:
            ctx.ProblemDetails.Status = (int)HttpStatusCode.InternalServerError;
            ctx.ProblemDetails.Detail = ex.Message;
            break;
    }

    var exceptionHandler = ctx.HttpContext?.Features.Get<IExceptionHandlerFeature>();

    if (exceptionHandler?.Endpoint != null)
    {
        var swaggerResponses = exceptionHandler.Endpoint.Metadata
            .Where(s => s.GetType() == typeof(Swashbuckle.AspNetCore.Annotations.SwaggerResponseAttribute))
            .Select(s => (Swashbuckle.AspNetCore.Annotations.SwaggerResponseAttribute)s)
            .ToList();

        if (!swaggerResponses.Where(s => s.StatusCode == (int)(ctx.ProblemDetails.Status ?? 400)).Any())
        {
            if (swaggerResponses.Where(s => s.StatusCode == (int)StatusCodes.Status409Conflict).Any())
            {
                ctx.ProblemDetails.Status = (int)HttpStatusCode.Conflict;
            }
            else
            {
                ctx.ProblemDetails.Status = (int)HttpStatusCode.BadRequest;
            }
        }
    }

    if (ctx.HttpContext != null)
    {
        ctx.HttpContext.Response.StatusCode =
            (int)(ctx.ProblemDetails.Status ?? (int)HttpStatusCode.BadRequest);
    }
});

var activeDevelopmentPage = builder.Configuration.GetSection("Debug").GetValue<bool?>("EnableDeveloperExceptionPage") ?? default(bool);
if (activeDevelopmentPage)
{
    //builder.Services.AddDatabaseDeveloperPageExceptionFilter();
}

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddAuthentication(BearerAuthenticationHandler.SchemeName)
    .AddScheme<AuthenticationSchemeOptions, BearerAuthenticationHandler>(BearerAuthenticationHandler.SchemeName, null);

builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    //[ALR] Ejemplo para versionado controlador
    options.Conventions.Controller<OrganizacionesController>().HasApiVersion(new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0));
    //options.Conventions.Controller<Proveedores2Controller>().HasApiVersion(new Microsoft.AspNetCore.Mvc.ApiVersion(2, 0));
});
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme.
                    Enter 'Bearer' [space] and then your token in the text input below.
                    Example: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
            Reference = new OpenApiReference
                {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
        new List<string>()
        }
    });

    c.SwaggerDoc("V1", new OpenApiInfo
    {
        Version = "V1.0",
        Title = "Arcante Organizaciones - OpenAPI 3.1 V1",
        Description = "Arcante Interfaces - OpenAPI 3.1 (ASP.NET Core 3.1) V1",
        Contact = new OpenApiContact()
        {
            Email = "soft.rf@moinsa.es",
            Name = "Moinsa",
            Url = new Uri("http://www.moinsa.es")
        }
    });
    c.UseInlineDefinitionsForEnums();
    
    c.ExampleFilters();

    //[ALR] Dejaremos que se genere un error para resolverlo
    //c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
    c.CustomSchemaIds(type => type.FullName);
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    c.IncludeXmlComments($"{AppContext.BaseDirectory}{Path.DirectorySeparatorChar}{xmlFilename}");

    //[FCP] Sustituye el nº de Version en la URL
    c.OperationFilter<RemoveVersionParameterFilter>();
    c.DocumentFilter<ReplaceVersionWithExactValueInPathFilter>();

    c.EnableAnnotations();

    //c.AddServer(new OpenApiServer()
    //{
    //    Url = "http://localhost:2222/api/v1"
    //});

    // Include DataAnnotation attributes on Controller Action parameters as Swagger validation rules (e.g required, pattern, ..)
    // Use [ValidateModelState] on Actions to actually validate it in C# as well!
    c.OperationFilter<GeneratePathParamsValidationFilter>();
    c.DocumentFilter<JsonPatchDocumentFilter>();

    c.OperationFilter<AcceptedLanguageHeader>();
    //Remove irrelevent schemas
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerExamplesFromAssemblies(Assembly.GetEntryAssembly());
builder.Services.AddSwaggerGen();


//builder.Services.AddProblemDetails();

var runningAsService = Utils.isRunningAsService(args);
if (runningAsService)
{
    Log.Information("Starting web host as windows service");
    builder.Host.ConfigureServices((context, services) =>
    {
        services.Configure<KestrelServerOptions>(context.Configuration.GetSection("Kestrel"));
    })
    .UseWindowsService();
}

#endregion Initializing Services

#region Initializing Middlewares


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseRequestLocalization(options =>
{
    //[ALR] If DefaultRequestCulture  is not invariant, we can have problems between different machines
    options.DefaultRequestCulture = new RequestCulture(CultureInfo.InvariantCulture);
    options.ApplyCurrentCultureToResponseHeaders = true;
    options.SupportedCultures = new List<CultureInfo>
    {
        new CultureInfo("en-US"),
        new CultureInfo("es-ES"),
        new CultureInfo("fr")
        // Agrega más culturas según tus necesidades
    };
    options.SupportedUICultures = new List<CultureInfo>
    {
        new CultureInfo("en-US"),
        new CultureInfo("es-ES"),
        new CultureInfo("fr")
        // Agrega más culturas según tus necesidades
    };
    options.RequestCultureProviders.Insert(0, new AcceptLanguageHeaderRequestCultureProvider());
});
app.UseHttpLogging();
app.UseHttpsRedirection();

app.UseRouting();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    var expandDoc = app.Configuration.GetValue<bool?>("Swagger:ExpandAll");
    if (expandDoc == null || expandDoc == true)
    {
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);

        //c.DefaultModelsExpandDepth(10);
        c.DefaultModelExpandDepth(10);
    }

    c.DefaultModelRendering(Swashbuckle.AspNetCore.SwaggerUI.ModelRendering.Model);

    //TODO: Either use the SwaggerGen generated Swagger contract (generated from C# classes)
    c.SwaggerEndpoint($"/swagger/V1/swagger.json", "V1.0");

    //TODO: Or alternatively use the original Swagger contract that's included in the static files
    // c.SwaggerEndpoint("/swagger-original.json", "Arcante Interfaces - OpenAPI 3.1 Original");
});

//[ALR] Configuramos CORS para admitir cualquier origen
//<see cref="member">https://jasonwatmore.com/post/2020/05/20/aspnet-core-api-allow-cors-requests-from-any-origin-and-with-credentials</see>
app.UseCors(builder => builder
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(hostname => true)
    .AllowCredentials()
);

//Middleware de Autenticacion y Autorizacion
app.UseAuthentication();
app.UseAuthorization();

app.UseExceptionHandler();
//Añade body de tipo ProblemDetails a respuestas con codigo 400 a 599 cuyo body fuese vacio
app.UseStatusCodePages();

if (app.Environment.IsDevelopment() && activeDevelopmentPage)
{
    //Añade informacion adicional a ProblemDetails para poder depurar errores en entornos de desarrollo
    app.UseDeveloperExceptionPage();
}

//Manejo automatico de transacciones
app.UseDbTransaction();

//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllers();
//});
app.MapControllers();

//Middleware de accepted-language
app.UseMiddleware<CustomHeaderValidatorMiddleware>(AcceptedLanguageHeader.HeaderName);

#endregion Initializing Middlewares

app.Run();

static void ManageSqlException(SqlException ex, ProblemDetails problemDetails)
{
    switch (ex.Number)
    {
        case 547:
            problemDetails.Status = (int)HttpStatusCode.Conflict;
            problemDetails.Detail = "Record cannot be deleted or changed as it is being used somewhere else.";
            break;

        case 2601:
        case 2627:
            problemDetails.Status = (int)HttpStatusCode.Conflict;
            problemDetails.Detail = "Record cannot be saved, as another record with this key already exists.";
            break;

        default:
            problemDetails.Status = (int)HttpStatusCode.InternalServerError;

            if (ex.Errors.Count > 0 && ex.Errors[0].Message.Contains("Mensaje:"))
            {
                var index = ex.Errors[0].Message.IndexOf("Mensaje:");
                problemDetails.Detail = ex.Errors[0].Message.Substring(index);
            }
            else
            {
                problemDetails.Detail = "Ups! Something went wrong. Try to repeat the operation in a few seconds. If the problem persists please contact with support.";
                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                {
                    problemDetails.Detail = $"{problemDetails.Detail}. Error: {ex.Errors[0].Message}";
                }
            }
            break;
    }
}
static void UpdateMigration()
{
    Log.Logger.Information($"Starting {nameof(UpdateMigration)} ");
    ApiCompanyDbContext context = null;
    string connection = "";
    try
    {
        connection= Utils.dbConnectionString();
        context = new ApiCompanyDbContext(connection, Utils.Configuration);

        var pendingMigration = context.Database.GetPendingMigrations();
        if (pendingMigration.Any())
        {
            context.Database.Migrate();
        }
    }
    catch (Exception ex)
    {
        Log.Logger.Error(ex, $"Error {nameof(UpdateMigration)}");
        throw new Exception($"Ocurrio un error al aplicar las actualizaciones a la base de datos, Error:\n{ex.Message}", ex);
    }
    finally
    {
        context?.Dispose();
        Log.Logger.Information($"End {nameof(UpdateMigration)}");
    }
}

static async Task<bool> MustBeStarted()
{
    var args = Environment.GetCommandLineArgs();
    const char doubleQuote = '\u0022';
    const string ServiceName = $"Organizaciones Rest API Service";

    if (args.Length >= 1)
    {
        string executablePath = $"{doubleQuote}{Path.Combine(AppContext.BaseDirectory, "Moinsa.Arcante.Company.Host.exe")}{doubleQuote} service";

        if (args.Contains("/Install"))
        {

            string binPath = @$"binPath={executablePath}";

            Log.Information("Installing Moinsa.Moinsa.Arcante.Company.Host.Host");
            await Cli.Wrap("sc")
                .WithArguments(new[] { "create", $"{ServiceName}", binPath, "start=delayed-auto" })
                .ExecuteAsync();
            await Cli.Wrap("sc")
                .WithArguments(new[] { "description", $"{ServiceName}", "Servicio API Rest para el manejo de Organizaciones" })
                .ExecuteAsync();
            await Cli.Wrap("sc")
                .WithArguments(new[] { "failure", $"{ServiceName}", "reset=86400", "actions=restart/restart/restart" })
                .WithValidation(CommandResultValidation.None)
            .ExecuteAsync();

            Log.Information($"Installed {ServiceName}");

            return false;
        }
        else if (args.Contains("/Uninstall"))
        {
            Log.Information("Uninstalling Moinsa.Moinsa.Arcante.Company.Host.Host");

            await Cli.Wrap("sc")
                .WithArguments(new[] { "stop", ServiceName })
                .WithValidation(CommandResultValidation.None)
                .ExecuteAsync();

            await Cli.Wrap("sc")
                .WithArguments(new[] { "delete", ServiceName })
                .ExecuteAsync();

            Log.Information($"Uninstalled {ServiceName}");

            return false;
        }
    }
    return true;
}
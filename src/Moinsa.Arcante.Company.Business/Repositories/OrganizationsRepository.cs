using API.Company.Business.Repositories.Interfaces;
using API.Company.Business.Validations;
using API.Company.Domain.Entities;
using API.Company.Domain.Enums;
using API.Company.Infraestructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text;
using System.Xml;
using static API.Company.Domain.Enums.EnumList;
using API.Company.Models.Exceptions;
using API.Company.Models.Companies;

namespace API.Company.Business.Repositories
{
    public class OrganizationsRepository : BaseRepository, IOrganizationsRepository
    {
     
        private readonly IProcesosRepository _procesosRepository;
        private readonly IConfiguration _configuration;

        public OrganizationsRepository(ApiCompanyDbContext dbContext,
                                       ILogger<OrganizationsRepository> logger,
                                       IProcesosRepository procesosRepository,
                                       AutoMapper.IMapper mapper,
                                       IConfiguration configuration) : base(dbContext, logger, mapper)
        {
            _procesosRepository = procesosRepository;
            _configuration = configuration;
        }
        public async Task<List<OrganizationsResponseModel>> GetOrganization(string? nameOrganization, string? applicationName)
        {
            try
            {      
                Logger.LogDebug($"Start repository {nameof(GetOrganization)}");

                var query = (from org in DbContext.Organizations
                              join app in DbContext.Applications on org.IdApplication equals app.Id into applist from app2 in applist.DefaultIfEmpty()
                              select new { org, app2 });                              
                if(!string.IsNullOrEmpty(nameOrganization)) 
                {
                    query=query.Where(o => o.org.Nombre == nameOrganization);
                }
                if (!string.IsNullOrEmpty(applicationName))
                {
                    query = query.Where(o => o.app2.Name == applicationName);
                }

                var result = await (from q in query
                                select new OrganizationsResponseModel
                                {
                                    Id = q.org.Id,
                                    Nombre = q.org.Nombre,
                                    CadenaConexion = q.org.CadenaConexion,
                                    Entorno = q.org.Entorno,
                                    Version = q.org.Version,
                                    Estado = q.org.Estado,
                                    IdAplication = q.app2.Id,
                                    Aplicacion = q.app2.Name
                                }).OrderByDescending(o => o.Id).ToListAsync();
                if (result.Any())
                {
                    foreach (var item in result)
                    {
                        //if (item.Estado != Utils.GetDisplayName(EnumList.listEstadosBBDD.OK))
                        //{
                            var estado = await CheckBBDD(item.Id, item.IdAplication, null);
                            item.Estado = estado;

                        //}
                    }
                }

                return this.Mapper.Map<List<OrganizationsResponseModel>>(result);
                
            }
            finally
            {
                Logger.LogDebug($"End repository {nameof(GetOrganization)} ");
            }
        }
        /// <summary>
        /// Metodo para comprobar si la base de datos esta inicalizada o no. Se ha generado con un switch para mas adelante identificar segun el source (sqlServer,Oracle..) 
        /// Dos formas de llamar al metodo, o bien conociendo la organizacion o si no la sabemos pasamos la cadena
        /// </summary>
        /// <param name="idOrganization"></param>
        /// <param name="idAplication"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        private async Task<string> CheckBBDD(int? idOrganization, int? idAplication, string? connectionString)
        {
            try
            {
                Logger.LogDebug($"Starting repository {nameof(CheckBBDD)}");

                var org = await DbContext.Organizations.FirstOrDefaultAsync(o => o.Id == idOrganization);
                var appSource = await DbContext.Applications.Where(o => o.Id == idAplication).Select(o => o.SourceUpdate).FirstOrDefaultAsync();
                var estadoBBDD = string.Empty;
                var cadena = org?.CadenaConexion ?? connectionString;
                if (!string.IsNullOrEmpty(cadena))
                {
                    try
                    {
                        switch (appSource)
                        {
                            case (int)EnumList.listSourceUpdate.SQLPackage:

                                //Comprobamos si la Cadena de conexion de la orgnaizacion que vamos a crear está inicializada o no.
                                SqlConnectionStringBuilder scb = new SqlConnectionStringBuilder(cadena);
                                scb.ConnectTimeout = 5;  // 5 seconds wait 0 = Infinite (better avoid)
                                cadena = scb.ToString();
                                using (var connection = new SqlConnection(cadena))
                                {
                                    connection.Open();
                                    estadoBBDD = Utils.GetDisplayName(EnumList.listEstadosBBDD.OK);
                                }
                                break;

                            default:
                                estadoBBDD = Utils.GetDisplayName(EnumList.listEstadosBBDD.SinInicializar);
                                break;
                        }
                    }
                    catch
                    {
                        estadoBBDD = Utils.GetDisplayName(EnumList.listEstadosBBDD.SinInicializar);
                    }
                }
                if (org != null)
                {
                    org.Estado = estadoBBDD;
                    await DbContext.SaveChangesAsync();
                }

                return estadoBBDD;
            }
            finally
            {
                Logger.LogDebug($"End repository {nameof(CheckBBDD)}");
            }
        }
        public async Task<Organizations> GetOrganizationByConn(string cadenaCon)
        {
            try
            {
                Logger.LogDebug($"Start repository {nameof(GetOrganization)}");

                var result = await DbContext.Organizations.FirstOrDefaultAsync(o => o.CadenaConexion == cadenaCon);
                
                return result;

            }
            finally
            {
                Logger.LogDebug($"End repository {nameof(GetOrganization)} ");
            }
        }        

        public async Task<OrganizationsResponseModel> AddOrganization(Organizations organization)
        {
            try
            {
                Logger.LogDebug($"Start repository {nameof(AddOrganization)}");
                Guard.CheckIsNotNull(organization, nameof(organization));
                Guard.CheckIsNotNullOrWhiteSpace(organization.Nombre, nameof(organization.Nombre));
                Guard.CheckIsNotNullOrWhiteSpace(organization.CadenaConexion, nameof(organization.CadenaConexion));

                var app = await DbContext.Applications.FirstOrDefaultAsync(o => o.Id==organization.IdApplication);
                if (app==null) { throw new ConflictException("La Aplicacion indicada no existe con ese nombre"); }

                var result = await DbContext.Organizations.FirstOrDefaultAsync(o => o.Nombre == organization.Nombre && 
                                        o.IdApplication == organization.IdApplication).ConfigureAwait(false);
                var estadoBBDD = string.Empty;

                if (result != null) 
                { 
                    throw new ConflictException("Ya existe una organizacion con ese nombre"); 
                }

                //Si la organizacion viene con estado Actualiando no comprobamos si esta inicializada
                if (string.IsNullOrEmpty(organization.Estado) || !organization.Estado.Equals(Utils.GetDisplayName(EnumList.listEstadosBBDD.Actualizado)))
                {
                   estadoBBDD= await CheckBBDD(null,app.Id,organization.CadenaConexion);
                }

                var org = new Organizations
                {
                    Nombre = organization.Nombre,
                    CadenaConexion = organization.CadenaConexion,
                    Version = "0",
                    Estado = estadoBBDD,
                    Entorno = organization.Entorno ?? string.Empty,
                    IdApplication = app.Id
                };
                await DbContext.AddAsync(org).ConfigureAwait(true);
                await DbContext.SaveChangesAsync().ConfigureAwait(true);

                return this.Mapper.Map<OrganizationsResponseModel>(org);

            }
            finally
            {
                Logger.LogDebug($"End repository {nameof(AddOrganization)} ");
            }
        }

        public async Task<bool> DeleteOrganization(int idOrganization)
        {
            try
            {
                Logger.LogDebug($"Starting repository {nameof(DeleteOrganization)} ");
                Guard.CheckGreaterThan(idOrganization, nameof(idOrganization), 0);
                var result = await DbContext.Organizations.FirstOrDefaultAsync(o => o.Id == idOrganization).ConfigureAwait(false);
                if (result != null)
                {
                    DbContext.Remove(result);
                    await DbContext.SaveChangesAsync().ConfigureAwait(true);
                    return true;
                }
                else
                {
                    throw new ConflictException($"No existe la organización con id [{idOrganization}].");
                }
            }
            finally
            {
                Logger.LogDebug($"End repository {nameof(DeleteOrganization)} ");
            }

        }

        private async Task<bool> PackageSqlDacpac(string con, OrganizationsResponseModel org, string rutaSQLPackage, string rutaDacPac,string profile,string version)
        {
            try
            {
                Logger.LogDebug($"Starting repository {nameof(PackageSqlDacpac)} ");
                var errs = new StringBuilder();
                var proc = new Process();
            
            
                await _procesosRepository.AddOrUpdateProceso(con, org.Id, (int)estadosProcesosEnum.EnProgreso, errs.ToString());
                //UpdateEstadoOrganizacionIdAsync(org.Id, "En Uso", version, org.Entorno, con);
                var rutaLog = $"./Logs/APIOrganizaciones_API_DACPAC_{org.Nombre}-.Log";
                string args = $"/Action:Publish /Profile:{rutaDacPac+profile} /SourceFile:{rutaDacPac}API.Db.dacpac /TargetConnectionString:\"{org.CadenaConexion}\" /Diagnostics:true /DiagnosticsFile:{rutaLog}";
                bool hayError = false;
                ProcessStartInfo startInfo = new ProcessStartInfo()
                {
                    FileName = rutaSQLPackage,
                    Arguments = args
                };
                startInfo.RedirectStandardOutput = true;
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;
                startInfo.RedirectStandardError = true;
                proc.StartInfo = startInfo;
               // proc.OutputDataReceived += (a, b) => Console.WriteLine(b.Data);
                proc.ErrorDataReceived += (a, b) => {
                    if (!hayError)
                    {
                        hayError = !String.IsNullOrEmpty(b.Data);
                    }
                    errs.Append(b.Data);
                };
                proc.Start();
                proc.BeginOutputReadLine();
                proc.BeginErrorReadLine();
                proc.WaitForExit();
                string stderr = errs.ToString();

                if(proc.ExitCode!=0|| hayError)
                {
                    Console.WriteLine(stderr);
                    await _procesosRepository.AddOrUpdateProceso(con, org.Id, (int)estadosProcesosEnum.Conflicto, stderr);
                    return false;
                }
                else 
                {
                    await _procesosRepository.AddOrUpdateProceso(con, org.Id, (int)estadosProcesosEnum.ProcesoTerminado, string.Empty);
                    return true;
                }
                
            }
            finally
            {
                UpdateEstadoOrganizacionIdAsync(org.Id, Utils.GetDisplayName(EnumList.listEstadosBBDD.OK), version, org.Entorno, con);
                Logger.LogDebug($"End repository {nameof(PackageSqlDacpac)} ");

            }
        }

        /// <summary>
        /// Creamos la organizacion y una vez creado vamos a crear la BBDD en esa ruta indicada
        /// </summary>
        /// <param name="organization"></param>
        /// <returns></returns>
        public async void CreateOrUpdateOrganizacionDDBB(OrganizationsResponseModel organization, string rutaSQLPackage, string rutaDacPac)
        {
            try
            {
                Logger.LogDebug($"Starting repository {nameof(CreateOrUpdateOrganizacionDDBB)} ");
                Guard.CheckIsNotNull(organization, nameof(organization));
                Guard.CheckIsNotNullOrWhiteSpace(rutaDacPac,nameof(rutaDacPac));
                Guard.CheckIsNotNullOrWhiteSpace(rutaSQLPackage,nameof(rutaSQLPackage));

                var conexionBBDD = DbContext.Database.GetDbConnection().ConnectionString;
                Guard.CheckIsNotNullOrWhiteSpace(conexionBBDD, nameof(conexionBBDD),"No se ha podido recuperar la cadena de conexion desde appSettings.json");

                string profile = "";


                switch (organization.Entorno)
                {
                    case "DEV":
                        profile = "00-DEV-API.Db.publish.xml";
                        break;
                    case "QA":
                        profile = "01-QA-API.Db.publish.xml";
                        break;
                    case "PRO":
                        profile = "02-PRO-API.Db.publish.xml";
                        break;

                    default:
                        profile = "00-DEV-API.Db.publish.xml";
                        organization.Entorno = "DEV";
                        break;
                }

                XmlDocument doc = new XmlDocument();
                doc.Load(rutaDacPac + profile);
                var version = doc.GetElementsByTagName("SqlCmdVariable")[10].InnerText;


                _=Task.Run(async () =>
                {
                    Thread.Sleep(2000);
                    UpdateEstadoOrganizacionIdAsync(organization.Id, organization.Estado, version, organization.Entorno, conexionBBDD);
                    await PackageSqlDacpac(conexionBBDD, organization, rutaSQLPackage, rutaDacPac, profile, version);
                });
            }
            finally
            {
                Logger.LogDebug($"End repository {nameof(CreateOrUpdateOrganizacionDDBB)} ");
            }

        }


        private bool UpdateEstadoOrganizacionIdAsync(int idOrg, string estado, string version, string entorno, string con)
        {
            try
            {
                using (ApiCompanyDbContext context = new ApiCompanyDbContext(con,_configuration))
                {
                    var organizacion = context.Organizations.FirstOrDefault(o => o.Id == idOrg);
                    if (organizacion != null)
                    {
                        organizacion.Version = version;
                        organizacion.Estado = estado;
                        organizacion.Entorno = entorno;

                        context.SaveChanges();
                    }

                }

                return true;
            }
            finally
            {

            }
        }

        public async Task<OrganizationsResponseModel> UpdateOrganization(int idOrganization, OrganizationsUpdateRequestModel organizationUpdate)
        {
            Logger.LogDebug($"Start repository {nameof(AddOrganization)}");
            Guard.CheckGreaterThan(idOrganization, nameof(idOrganization),0);
           
            var originalOrg = await DbContext.Organizations.FirstOrDefaultAsync(o=>o.Id== idOrganization);
            if (originalOrg == null)
            {
                throw new NotFoundException($"No existe la organizacion con id [{0}]");
            }



            //recuperamos al app a la que queremos cambiar
            if (!string.IsNullOrEmpty(organizationUpdate.Aplicacion))
            {
                var appId = await DbContext.Applications.Where(o => o.Name.Equals(organizationUpdate.Aplicacion)).Select(o => o.Id).FirstOrDefaultAsync();
                if (appId == 0) { throw new NotFoundException("La Aplicacion indicada no existe con ese nombre"); }
                originalOrg.IdApplication= appId;

            }
            var appName = await DbContext.Applications.Where(o => o.Id == originalOrg.IdApplication).Select(o => o.Name).FirstOrDefaultAsync();
            if (!string.IsNullOrEmpty(organizationUpdate.Nombre))
            {
                var newNameOrg = await GetOrganization(organizationUpdate.Nombre, appName);
                if (newNameOrg != null && newNameOrg[0].Nombre!=originalOrg.Nombre) { throw new ConflictException($"El nombre que intenta usar [{organizationUpdate.Nombre}] ya esta siendo usado para la aplicación [{appName}]"); }

                originalOrg.Nombre= organizationUpdate.Nombre;
            }

            if (!string.IsNullOrEmpty(organizationUpdate.CadenaConexion))
            {
                var orgCadena = await GetOrganizationByConn(organizationUpdate.CadenaConexion);
                if ( orgCadena != null && orgCadena.CadenaConexion!= originalOrg.CadenaConexion) { throw new NotFoundException("La cadena de conexion ya esta siendo usada"); }

                originalOrg.CadenaConexion = organizationUpdate.CadenaConexion;
            }

            if (!string.IsNullOrEmpty(organizationUpdate.Entorno))
            {
                originalOrg.Entorno = organizationUpdate.Entorno;
            }

            if (!string.IsNullOrEmpty(organizationUpdate.Version))
            {
                originalOrg.Version = organizationUpdate.Version;
            }

            //Si la organizacion viene con estado Actualiando no comprobamos si esta inicializada
            var estadoBBDD =  await CheckBBDD(originalOrg.Id, originalOrg.IdApplication, null);
            
            originalOrg.Estado=estadoBBDD;

            await DbContext.SaveChangesAsync().ConfigureAwait(true);
            var result=  this.Mapper.Map<OrganizationsResponseModel>(originalOrg);
            result.Aplicacion = appName;

            return result;

        }
    }
}

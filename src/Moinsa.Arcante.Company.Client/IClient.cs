//----------------------
// <auto-generated>
//     Generated using the NSwag toolchain v14.0.3.0 (NJsonSchema v11.0.0.0 (Newtonsoft.Json v13.0.0.0)) (http://NSwag.org)
// </auto-generated>
//----------------------

using Microsoft.AspNetCore.Mvc;
using API.Company.Models;
using API.Company.Models.Process;
using API.Company.Models.Companies;
using Microsoft.Extensions.Configuration;
using API.Company.Models.Applications;

#pragma warning disable 108 // Disable "CS0108 '{derivedDto}.ToJson()' hides inherited member '{dtoBase}.ToJson()'. Use the new keyword if hiding was intended."
#pragma warning disable 114 // Disable "CS0114 '{derivedDto}.RaisePropertyChanged(String)' hides inherited member 'dtoBase.RaisePropertyChanged(String)'. To make the current member override that implementation, add the override keyword. Otherwise add the new keyword."
#pragma warning disable 472 // Disable "CS0472 The result of the expression is always 'false' since a value of type 'Int32' is never equal to 'null' of type 'Int32?'
#pragma warning disable 612 // Disable "CS0612 '...' is obsolete"
#pragma warning disable 1573 // Disable "CS1573 Parameter '...' has no matching param tag in the XML comment for ...
#pragma warning disable 1591 // Disable "CS1591 Missing XML comment for publicly visible type or member ..."
#pragma warning disable 8073 // Disable "CS8073 The result of the expression is always 'false' since a value of type 'T' is never equal to 'null' of type 'T?'"
#pragma warning disable 3016 // Disable "CS3016 Arrays as attribute arguments is not CLS-compliant"
#pragma warning disable 8603 // Disable "CS8603 Possible null reference return"
#pragma warning disable 8604 // Disable "CS8604 Possible null reference argument for parameter"
#pragma warning disable 8625 // Disable "CS8625 Cannot convert null literal to non-nullable reference type"

namespace Microsoft.AspNetCore.Mvc
{
    using System = global::System;

    [System.CodeDom.Compiler.GeneratedCode("NSwag", "14.0.3.0 (NJsonSchema v11.0.0.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial interface IClient
    {
        /// <summary>
        /// Recupera las organizaciones
        /// </summary>
        /// <returns>Success</returns>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<System.Collections.ObjectModel.ObservableCollection<ApplicationsResponseModels>> AplicacionesGetAsync(string nombreAplicacion, string accept_language);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>
        /// Recupera las organizaciones
        /// </summary>
        /// <returns>Success</returns>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<System.Collections.ObjectModel.ObservableCollection<ApplicationsResponseModels>> AplicacionesGetAsync(string nombreAplicacion, string accept_language, System.Threading.CancellationToken cancellationToken);

        /// <summary>
        /// Inserta una Aplicacion
        /// </summary>
        /// <returns>Created</returns>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<ApplicationsResponseModels> AplicacionesPostAsync(string accept_language, ApplicationsRequestModels body);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>
        /// Inserta una Aplicacion
        /// </summary>
        /// <returns>Created</returns>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<ApplicationsResponseModels> AplicacionesPostAsync(string accept_language, ApplicationsRequestModels body, System.Threading.CancellationToken cancellationToken);

        /// <summary>
        /// Borra una aplicacion filtrando por nombre
        /// </summary>
        /// <returns>Success</returns>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        System.Threading.Tasks.Task AplicacionesDeleteAsync(string nombreAplicacion, string accept_language);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>
        /// Borra una aplicacion filtrando por nombre
        /// </summary>
        /// <returns>Success</returns>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        System.Threading.Tasks.Task AplicacionesDeleteAsync(string nombreAplicacion, string accept_language, System.Threading.CancellationToken cancellationToken);

        /// <summary>
        /// Recuperamos los datos de la organización filtrando por su nombre.
        /// </summary>
        /// <param name="nombreOrganizacion">Nombre de la organización por el que vamos a realizar la búsqueda</param>
        /// <returns>Success</returns>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<System.Collections.ObjectModel.ObservableCollection<OrganizationsResponseModel>> OrganizacionesGetAsync(string nombreOrganizacion, string nombreAplicacion, string accept_language);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>
        /// Recuperamos los datos de la organización filtrando por su nombre.
        /// </summary>
        /// <param name="nombreOrganizacion">Nombre de la organización por el que vamos a realizar la búsqueda</param>
        /// <returns>Success</returns>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<System.Collections.ObjectModel.ObservableCollection<OrganizationsResponseModel>> OrganizacionesGetAsync(string nombreOrganizacion, string nombreAplicacion, string accept_language, System.Threading.CancellationToken cancellationToken);

        /// <summary>
        /// Insertamos una nueva organización
        /// </summary>
        /// <returns>Created</returns>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<OrganizationsResponseModel> OrganizacionesPostAsync(string accept_language, OrganizationsRequestModel body);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>
        /// Insertamos una nueva organización
        /// </summary>
        /// <returns>Created</returns>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<OrganizationsResponseModel> OrganizacionesPostAsync(string accept_language, OrganizationsRequestModel body, System.Threading.CancellationToken cancellationToken);

        /// <summary>
        /// Borramos los datos de la organización por su nombre
        /// </summary>
        /// <returns>Success</returns>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        System.Threading.Tasks.Task OrganizacionesDeleteAsync(string nombreOrganizacion, string nombreAplicacion, string accept_language);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>
        /// Borramos los datos de la organización por su nombre
        /// </summary>
        /// <returns>Success</returns>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        System.Threading.Tasks.Task OrganizacionesDeleteAsync(string nombreOrganizacion, string nombreAplicacion, string accept_language, System.Threading.CancellationToken cancellationToken);

        /// <summary>
        /// Insertamos una nueva organización y si no existe la base de datos se crea siguiendo esa cadena de conexión.
        /// <br/>El nombre es con el que se guarda la organización y no con el que se crea la bases de datos que se escoge de la cadena.
        /// </summary>
        /// <returns>Created</returns>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<OrganizationsResponseModel> OrganizacionesCreateorganizacionddbbAsync(string accept_language, OrganizationsCreateDDBBRequestModel body);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>
        /// Insertamos una nueva organización y si no existe la base de datos se crea siguiendo esa cadena de conexión.
        /// <br/>El nombre es con el que se guarda la organización y no con el que se crea la bases de datos que se escoge de la cadena.
        /// </summary>
        /// <returns>Created</returns>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<OrganizationsResponseModel> OrganizacionesCreateorganizacionddbbAsync(string accept_language, OrganizationsCreateDDBBRequestModel body, System.Threading.CancellationToken cancellationToken);

        /// <summary>
        /// Metodo para actualizar la base de datos.
        /// </summary>
        /// <returns>Created</returns>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<OrganizationsResponseModel> OrganizacionesUpdateorganizacionddbbAsync(string accept_language, OrganizationsCreateDDBBRequestModel body);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>
        /// Metodo para actualizar la base de datos.
        /// </summary>
        /// <returns>Created</returns>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<OrganizationsResponseModel> OrganizacionesUpdateorganizacionddbbAsync(string accept_language, OrganizationsCreateDDBBRequestModel body, System.Threading.CancellationToken cancellationToken);

        /// <summary>
        /// Recuperamos el Proceso en el que se encuentra organizacion filtrando por id.
        /// </summary>
        /// <returns>Success</returns>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<ProcesosResponseModel> ProcesosGetprocesoAsync(int? idOrganizacion, string accept_language);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>
        /// Recuperamos el Proceso en el que se encuentra organizacion filtrando por id.
        /// </summary>
        /// <returns>Success</returns>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<ProcesosResponseModel> ProcesosGetprocesoAsync(int? idOrganizacion, string accept_language, System.Threading.CancellationToken cancellationToken);

        /// <summary>
        /// Obtiene un Jwt token para ser utilizado en posteriores llamados al web api.
        /// <br/>Son requeridos usuario y contraseña.
        /// </summary>
        /// <returns>Success</returns>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<string> TokenAuthenticateAsync(string accept_language, CredentialsModel body);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>
        /// Obtiene un Jwt token para ser utilizado en posteriores llamados al web api.
        /// <br/>Son requeridos usuario y contraseña.
        /// </summary>
        /// <returns>Success</returns>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<string> TokenAuthenticateAsync(string accept_language, CredentialsModel body, System.Threading.CancellationToken cancellationToken);

    }

    



    [System.CodeDom.Compiler.GeneratedCode("NSwag", "14.0.3.0 (NJsonSchema v11.0.0.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class SwaggerException : System.Exception
    {
        public int StatusCode { get; private set; }

        public string Response { get; private set; }

        public System.Collections.Generic.IReadOnlyDictionary<string, System.Collections.Generic.IEnumerable<string>> Headers { get; private set; }

        public SwaggerException(string message, int statusCode, string response, System.Collections.Generic.IReadOnlyDictionary<string, System.Collections.Generic.IEnumerable<string>> headers, System.Exception innerException)
            : base(message + "\n\nStatus: " + statusCode + "\nResponse: \n" + ((response == null) ? "(null)" : response.Substring(0, response.Length >= 512 ? 512 : response.Length)), innerException)
        {
            StatusCode = statusCode;
            Response = response;
            Headers = headers;
        }

        public override string ToString()
        {
            return string.Format("HTTP Response: \n\n{0}\n\n{1}", Response, base.ToString());
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("NSwag", "14.0.3.0 (NJsonSchema v11.0.0.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class SwaggerException<TResult> : SwaggerException
    {
        public TResult Result { get; private set; }

        public SwaggerException(string message, int statusCode, string response, System.Collections.Generic.IReadOnlyDictionary<string, System.Collections.Generic.IEnumerable<string>> headers, TResult result, System.Exception innerException)
            : base(message, statusCode, response, headers, innerException)
        {
            Result = result;
        }
    }

}

#pragma warning restore  108
#pragma warning restore  114
#pragma warning restore  472
#pragma warning restore  612
#pragma warning restore 1573
#pragma warning restore 1591
#pragma warning restore 8073
#pragma warning restore 3016
#pragma warning restore 8603
#pragma warning restore 8604
#pragma warning restore 8625
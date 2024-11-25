
using System;
namespace Moinsa.Arcante.Company.Models.Companies
{
    public partial class OrganizationsResponseModel : OrganizationsRequestModel
    {
        /// <summary>
        /// Código único.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Version en la que se encuentra
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// Estado en el que se encuentra la organizacion
        /// </summary>
        public string Estado { get; set; }
        /// <summary>
        /// Entorno en le que esta creada la BBDD de la organizacion.
        /// </summary>
        public string Entorno { get; set; }
        public int IdAplication { get; set; }      
    }
}

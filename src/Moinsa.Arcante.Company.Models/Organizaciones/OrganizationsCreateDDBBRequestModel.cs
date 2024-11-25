
using System;
using System.ComponentModel;

namespace Moinsa.Arcante.Company.Models.Companies
{
    public partial class OrganizationsCreateDDBBRequestModel : OrganizationsRequestModel
    {
        /// <summary>
        /// Código único.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Version en la que se encuentra
        /// </summary>
        [DefaultValue("0")]
        public string Version { get; set; }
        /// <summary>
        /// Estado de la organizacion en la que se encuentra actualmente, pudiendo ser: "No Inicializada", "Ok" o "Actualizando"
        /// </summary>
        [DefaultValue("No Creada")]
        public string Estado { get; set; }

        /// <summary>
        /// Entorno en el que se publica la BBDD
        /// </summary>
        [DefaultValue("")]
        public string Entorno { get; set; }

    }
}
    


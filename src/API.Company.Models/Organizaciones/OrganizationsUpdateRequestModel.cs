namespace API.Company.Models.Companies
{
    public class OrganizationsUpdateRequestModel
    {

        /// <summary>
        /// Nombre de la organización.
        /// </summary>
        public string Nombre { get; set; }
        /// <summary>
        /// Cadena de conexión perteneciente a la organización (Está encriptada)
        /// </summary>
        public string CadenaConexion { get; set; }
        /// <summary>
        /// Aplicacion a la que pertenece
        /// </summary>
        public string Aplicacion { get; set; }
        /// <summary>
        /// Version en la que se encuentra
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// Entorno en le que esta creada la BBDD de la organizacion.
        /// </summary>
        public string Entorno { get; set; }
    }

}
namespace API.Company.Models.Companies
{
    public partial class OrganizationsRequestModel
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
    }

}
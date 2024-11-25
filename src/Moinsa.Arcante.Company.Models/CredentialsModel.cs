
namespace API.Company.Models
{
    /// <summary>
    /// Objecto para indicar las credenciales de acceso
    /// </summary>
    public class CredentialsModel
    {
        /// <summary>
        /// Nombre de acceso
        /// </summary>
        /// <example>1234</example>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Contraseña del acceso
        /// </summary>
        /// <example>1234</example>
        public string Password { get; set; } = string.Empty;


    }

}
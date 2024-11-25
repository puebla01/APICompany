namespace API.Company.Host.Enums
{
    public class StatusCodesDescriptions
    {
        public const string Status200OK = "Operacion Completada";
        public const string Status201Created = "Creado";
        public const string Status400BadRequest = "Petición Incorrecta";
        public const string Status401Unauthorized = "Autorización Incorrecta";
        public const string Status404NotFound = "No encontrado";
        public const string Status409Conflict = "Conflicto";
        public const string Status500InternalServerError = "Error Interno del Servidor";
    }
}

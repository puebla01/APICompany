using Microsoft.Extensions.Localization;

namespace Moinsa.Arcante.Company.Business.Validations
{
    public static class HelperResources
    {
        /// <summary>
        /// Obtiene la cadena formateada del archivo de recursos. Mirar ejemplo.
        /// </summary>
        /// <param name="localizer">String localizer para acceder al recurso.</param>
        /// <param name="ID">Se debe pasar el ID del archivo de recursos. Si se utiliza un archivo autogenerado debemos hacer uso del "NameOf" para acceder al recurso</param>
        /// <param name="args">Valores para añadir en el texto como parametros.</param>
        /// <returns>Devuelve una cadena localizada</returns>
        /// <example> HelperResources.GetString(
        ///        this.Localizer,
        ///        nameof(BusinessResources.InputOrdersRepository_Shared_TipoEntradaNoExiste),
        ///        order.TipoDeEntrada, order.Almacen);
        /// </example>

        public static string GetString(IStringLocalizer localizer,
            string ID,
            params object[] args)
        {
            return string.Format(localizer.GetString(ID), args);
        }
    }
}

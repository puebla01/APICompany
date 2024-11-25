using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Xml.Linq;

namespace Moinsa.Arcante.Company.Domain.Enums
{

    public class EnumList
    {

        public enum estadosProcesosEnum
        {
            [Display(Name = "En Progreso")] EnProgreso = 0,
            [Display(Name = "Proceso Terminado")] ProcesoTerminado = 1,
            [Display(Name = "Conflicto")] Conflicto = 2
        };

        public enum listEstadosBBDD 
        {
            [Display(Name = "Sin Inicializar")] SinInicializar = 0,
            [Display(Name = "OK")] OK= 1,
            [Display(Name = "Actualizando")] Actualizado= 2,
          
        };
        public enum listSourceUpdate
        {
            [Display(Name = "Ninguna")] Ninguna=0,
            [Display(Name = "SQL Package")] SQLPackage = 1,
            [Display(Name = "Entity Framework")] EntityFramework = 2,
        };
    }
}

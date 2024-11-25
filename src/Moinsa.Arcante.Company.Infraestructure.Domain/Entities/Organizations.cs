using System;
using System.Collections.Generic;

namespace Moinsa.Arcante.Company.Domain.Entities
{
    public partial class Organizations
    {
        public Organizations()
        {
            Procesos = new HashSet<Procesos>();
        }

        public int Id { get; set; }
        public string Nombre { get; set; }
        public string CadenaConexion { get; set; }
        public DateTime Fxaltareg { get; set; }
        public DateTime Fxmodreg { get; set; }
        public string Entorno { get; set; }
        public string Estado { get; set; }
        public string Version { get; set; }
        public int IdApplication { get; set; }

        public virtual Applications IdApplicationNavigation { get; set; }
        public virtual ICollection<Procesos> Procesos { get; set; }
    }
}

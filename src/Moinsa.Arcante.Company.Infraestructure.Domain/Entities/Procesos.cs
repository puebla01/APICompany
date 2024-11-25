using System;
using System.Collections.Generic;

namespace Moinsa.Arcante.Company.Domain.Entities
{
    public partial class Procesos
    {
        public int Id { get; set; }
        public string Proceso { get; set; }
        public int IdOrganizacion { get; set; }
        public string Obs { get; set; }

        public virtual Organizations IdOrganizacionNavigation { get; set; }
    }
}

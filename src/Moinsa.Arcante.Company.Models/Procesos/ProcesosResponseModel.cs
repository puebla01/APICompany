using System;
using System.Collections.Generic;
using System.Text;

namespace Moinsa.Arcante.Company.Models.Process
{
    public class ProcesosResponseModel
    {
        public int Id { get; set; }
        public int OrganizacionId { get; set; }

        public string Organizacion { get; set; }
        public string estadoProceso { get; set; }
    }
}

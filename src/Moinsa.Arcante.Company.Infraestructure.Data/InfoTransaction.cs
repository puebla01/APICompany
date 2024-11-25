using System;
using System.Collections.Generic;
using System.Text;

namespace Moinsa.Arcante.Company.Infraestructure.Data
{
    public class InfoTransaction
    {
        public bool IsNewTransaction { get; set; }
        public string NameClassOrigin { get; set; }
        public string NameMethodOrigin { get; set; }
    }
}

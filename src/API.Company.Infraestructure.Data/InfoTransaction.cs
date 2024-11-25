using System;
using System.Collections.Generic;
using System.Text;

namespace API.Company.Infraestructure.Data
{
    public class InfoTransaction
    {
        public bool IsNewTransaction { get; set; }
        public string NameClassOrigin { get; set; }
        public string NameMethodOrigin { get; set; }
    }
}

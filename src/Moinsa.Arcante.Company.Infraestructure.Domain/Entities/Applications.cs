using System;
using System.Collections.Generic;

namespace API.Company.Domain.Entities
{
    public partial class Applications
    {
        public Applications()
        {
            Organizations = new HashSet<Organizations>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int SourceUpdate { get; set; }
        public DateTime Fxaltareg { get; set; }
        public DateTime Fxmodreg { get; set; }

        public virtual ICollection<Organizations> Organizations { get; set; }
    }
}

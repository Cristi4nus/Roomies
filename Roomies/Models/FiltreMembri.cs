using System;
using System.Collections.Generic;
using System.Text;

namespace Roomies.Models
{
    public class FiltreMembri
    {
        public string Gen { get; set; }
        public string ZonaPreferata { get; set; }

        public int? BugetMin { get; set; }
        public int? BugetMax { get; set; }

        public string StilDeViata { get; set; }
        public string PreferintaDeTrai { get; set; }
    }
}


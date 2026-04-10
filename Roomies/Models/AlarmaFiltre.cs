using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Roomies.Models
{
    public class AlarmaFiltre
    {
            [PrimaryKey, AutoIncrement]
            public int Id { get; set; }
            public int UserId { get; set; }
            public string Gen { get; set; }
            public string Zona { get; set; }
            public string Buget { get; set; }
            public string StilViata { get; set; }
            public string Preferinte { get; set; }

    }
}

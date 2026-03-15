using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Roomies
{
    public class Membru
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Avatar { get; set; }
        public string Nume { get; set; }
        public string Prenume { get; set; }

        [Display(Name = "Parola")]
        [DataType(DataType.Password)]
        public string Parola { get; set; }
        public byte[] ParolaHash { get; set; }
        public byte[] ParolaSalt { get; set; }
        public int Varsta { get; set; }
        public string Gen { get; set; }
        public string Facultate { get; set; }
        public string NumarTelefon { get; set; }
        public string ZonaPreferata { get; set; }
        public int BugetMaxim { get; set; }
        public string PerioadaDeSedere { get; set; }
        public string StilDeViata { get; set; }
        public string PreferinteDeTrai { get; set; }
        public string Descriere { get; set; }
        
        [Unique]
        [Display(Prompt = "example@mail.com", Name = "Email")]
        public string Email { get; set; }
    }

}

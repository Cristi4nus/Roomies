using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace Roomies
{
    public class Prietenie
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public int User1Id { get; set; }
        public int User2Id { get; set; }
    }
}


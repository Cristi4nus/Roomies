using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Roomies
{
    public class Notificare
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int UserId { get; set; }

        public string Text { get; set; }

        public DateTime Data { get; set; }
    }

}

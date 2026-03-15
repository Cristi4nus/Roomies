using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Roomies
{
    public class CererePrietenie
    {
        [PrimaryKey,AutoIncrement]
        public int ID { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string Mesaj { get; set; }
        public string Status { get; set; }
        public DateTime Data { get; set; }
    }
}

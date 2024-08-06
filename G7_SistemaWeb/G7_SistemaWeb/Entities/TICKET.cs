using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Web;

namespace G7_SistemaWeb.Entities
{
    public class TICKET
    {
        public long ID_TICKET { get; set; }
        public string TITLE { get; set; }
        public string DESC_TICKET { get; set; }
        public long ID_STATE { get; set; }
        public int ACTIVE { get; set; }
        public string USU_OPEN { get; set; }
        public string USU_CHECK { get; set; }
        public string USU_APROVE { get; set; }
        public string USU_ARCHIVE { get; set; }
        public string USU_DELETE { get; set; }
        public string USU_COMPLETE { get; set; }

        public virtual STATE_TICKET STATE_TICKET { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
        public virtual User User2 { get; set; }
        public virtual User User3 { get; set; }
        public virtual User User4 { get; set; }
        public virtual User User5 { get; set; }
    }

    public class TicketAnswer
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public TICKET info { get; set; }
    }
}
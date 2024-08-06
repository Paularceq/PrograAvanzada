using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace G7_SistemaWeb.Entities
{
    public class STATE_TICKET
    {
        public STATE_TICKET()
        {
            this.TICKETs = new HashSet<TICKET>();
        }

        public long ID_STATE { get; set; }
        public string NAME_STATE { get; set; }
        public string DESC_STATE { get; set; }
        public int ACTIVE { get; set; }

        public virtual ICollection<TICKET> TICKETs { get; set; }
    }
}
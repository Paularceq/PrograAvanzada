using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace G7_SistemaWeb.Entities
{
    public class ROLE
    {
        public ROLE()
        {
            this.Users = new HashSet<User>();
        }

        public long ID_ROLE { get; set; }
        public string NAME_ROLE { get; set; }
        public string DESC_ROLE { get; set; }
        public int ACTIVE { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
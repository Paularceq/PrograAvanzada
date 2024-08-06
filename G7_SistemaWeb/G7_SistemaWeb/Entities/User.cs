using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace G7_SistemaWeb.Entities
{
    public class User
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
            this.TICKETs = new HashSet<TICKET>();
            this.TICKETs1 = new HashSet<TICKET>();
            this.TICKETs2 = new HashSet<TICKET>();
            this.TICKETs3 = new HashSet<TICKET>();
            this.TICKETs4 = new HashSet<TICKET>();
            this.TICKETs5 = new HashSet<TICKET>();
        }

        public string USU_ID { get; set; }
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public string EMAIL { get; set; }
        public string PASSWD { get; set; }
        public string TEMP_PASSWD { get; set; }
        public int ACTIVE { get; set; }
        public long ID_ROLE { get; set; }

        public virtual ROLE ROLE { get; set; }
        public virtual ICollection<TICKET> TICKETs { get; set; }
        public virtual ICollection<TICKET> TICKETs1 { get; set; }
        public virtual ICollection<TICKET> TICKETs2 { get; set; }
        public virtual ICollection<TICKET> TICKETs3 { get; set; }
        public virtual ICollection<TICKET> TICKETs4 { get; set; }
        public virtual ICollection<TICKET> TICKETs5 { get; set; }
    }

    public class UserAnswer
    {
        public User user { get; set; }
        public string Codigo { get; set; }
        public string Mensaje { get; set; }
        public string Token { get; set; }
    }
}
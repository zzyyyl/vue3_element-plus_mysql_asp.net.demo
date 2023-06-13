using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Tables
{
    /*
   pid                  int not null  comment '',
   pname                char(255)  comment '',
   psource              char(255)  comment '',
   pyear                date  comment '',
   ptype                int  comment '',
   level                int  comment '',
     */
    public class Paper
    {
        public int Pid { get; set; }
        public string? Pname { get; set; }
        public string? Psource { get; set; }
        [Column(TypeName = "DateTime")]
        public string? Pyear { get; set; }
        public int? Ptype { get; set; }
        public int? Level { get; set; }

        public override string ToString()
        {
            return $"pid:{Pid},pname:{Pname},psource:{Psource},pyear:{Pyear},ptype:{Ptype},level:{Level}";
        }
    }
}
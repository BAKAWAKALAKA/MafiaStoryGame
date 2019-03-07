using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBDBCommunication
{
    public class message
    {
        [Key]
        public int message_id { get; set; }
        public int from { get; set; }
        public int date { get; set; }
        public int chat { get; set; }
        public int forward_date { get; set; }
        public string text { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBDBCommunication
{
    [Table("messages", Schema = "main")]
    public class message
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int message_id { get; set; }
        public int user_id { get; set; }
        public int date { get; set; }
        public int chat_id { get; set; }
        public string text { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.SQLite;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace TBDBCommunication
{
    public class TBDBContext : DbContext
    {
        public TBDBContext(): base("DefaultConnection")
        {
        }

        public DbSet<user> users { get; set; }
        public DbSet<message> messages { get; set; }
    }
}

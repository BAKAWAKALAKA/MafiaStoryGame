using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBDBCommunication;
using System.Data.SQLite;
using System.Data.Entity;
namespace TelegramTests
{
    public class DBCommunicationTest
    {

        public static void Do()
        {
            using (var ctx = new TBDBContext())
            {
                var user = ctx.users.AsEnumerable();
                var c = user.Count();
            }
        }
    }
}

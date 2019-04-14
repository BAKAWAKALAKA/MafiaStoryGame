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
                var top = new List<message>();
                top.Add(new message() { chat_id = 1908, date = 12412, message_id = 1, text = "wf", user_id = 14 });
                top.Add(new message() { chat_id = 108, date = 1412, message_id = 2, text = "fwf", user_id = 809 });
                ctx.messages.AddRange(top);
                var tt = ctx.SaveChanges();
                ctx.messages.RemoveRange(top);
                var ttt = ctx.SaveChanges();
                var pp = new List<message>();
                pp.Add(new message() { chat_id = 18, date = 12, message_id = 1, text = "o0wf", user_id = 1 });
                ctx.messages.AddRange(pp);
                ctx.SaveChanges();


            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramAPI
{
    public interface CommandHandler
    {
        bool CanRespond(dynamic message);
       IEnumerable<Message> SendResponce(Message message);
    }
}

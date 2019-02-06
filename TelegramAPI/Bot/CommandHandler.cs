using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramAPI
{
    public interface CommandHandler
    {
        bool CanRespond(Messege message);
       IEnumerable<Messege> SendResponce(Messege message);
    }
}

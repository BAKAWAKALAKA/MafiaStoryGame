using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramAPI;

namespace TelegramBotService.Bot
{
    public class IdleCommand : CommandHandler
    {
        public bool CanRespond(Messege message)
        {
            ///todo проверить не написана ли определенная команда
            return message.text.StartsWith("/");
        }

        public IEnumerable<Messege> SendResponce(Messege message)
        {
            /// todo найти слово в словаре
            var messeges = new List<Messege>();
            // сначала нужно понять написано хираганой или на русском
            // потом нходим в словаре 
            /// либо просто диктионари сначала по ключам(яп) потом по значениям(русский)
            return messeges;
        }
    }
}

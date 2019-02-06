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
           return true;
        }

        public IEnumerable<Messege> SendResponce(Messege message)
        {
            var msg = message;
            return new Messege[] {new Messege() {
                chat = msg.chat,
                text = $"{msg.from.first_name} написал мне: {msg.text}" } };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramAPI;
using MafiaStoryGame;

namespace TelegramBotService.Bot
{
    public class HelpCommand : CommandHandler
    {
        public bool CanRespond(Messege message)
        {
            return message.text.Contains("/help");
        }

        public IEnumerable<Messege> SendResponce(Messege message)
        {
            var msg = new Messege()
            {
                chat = message.chat,
                text = "Привет! тут должен был быть хелпер который помог бы разобраться в командах но мне лень"
            };

            return new Messege[] { msg };
        }
    }
}

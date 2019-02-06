using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramAPI;
using MafiaStoryGame;

namespace TelegramBotService.Bot
{
    public class FreeRoomsCommand : CommandHandler
    {
        public bool CanRespond(Messege message)
        {
            return message.text.Contains("/free");
        }

        public IEnumerable<Messege> SendResponce(Messege message)
        {
            var msg = new Messege() { chat = message.chat };

            msg.text = GameManager.SeeFreeRoomsInfo();

            return new Messege[] { msg };
        }
    }
}

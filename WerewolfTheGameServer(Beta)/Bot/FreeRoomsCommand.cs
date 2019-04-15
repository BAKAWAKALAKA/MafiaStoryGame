using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramAPI;
using MafiaStoryGame;

namespace WerewolfTheGameServer.Bot
{
    public class FreeRoomsCommand : CommandHandler
    {
        public bool CanRespond(TelegramAPI.Messege message)
        {
            return message.text.ToLower().StartsWith("/free");
        }

        public IEnumerable<TelegramAPI.Messege> SendResponce(TelegramAPI.Messege message)
        {
            var msg = new TelegramAPI.Messege() { chat = message.chat };

            msg.text = GameManager.SeeFreeRoomsInfo();

            return new TelegramAPI.Messege[] { msg };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramAPI;
using MafiaStoryGame;

namespace TelegramBotService.Bot
{
    public class LeaveCommand : CommandHandler
    {
        public bool CanRespond(Messege message)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Messege> SendResponce(Messege message)
        {
            throw new NotImplementedException();
        }
    }
}

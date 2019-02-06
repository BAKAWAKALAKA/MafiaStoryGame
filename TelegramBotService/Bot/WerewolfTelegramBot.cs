using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramAPI;

namespace TelegramBotService.Bot
{
    public class WerewolfTelegramBot : TelegramBot
    {
        public List<CommandHandler> NotAuthcmds;
       // public List<CommandHandler> NotAuthcmds;

        public WerewolfTelegramBot(CommandHandler[] commands) : base(commands)
        {


        }



    }
}

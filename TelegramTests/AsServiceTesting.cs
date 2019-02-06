using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramAPI;
using TelegramBotService.Bot;

namespace TelegramTests
{
   public  class AsServiceTesting
    {

        public static void Do()
        {
            var bot = new TlgrBot(new CommandHandler[] { new CreateRoomCommand(), new JoinCommand(), new HelpCommand(), new FreeRoomsCommand() });
            while (true)
            {
                Console.ReadLine();
            }
        }

    }


    public class TlgrBot : TelegramBot
    {
        private


        public TlgrBot(CommandHandler[] commands) : base(commands)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramAPI;
using TelegramBotService.Bot;

namespace WerewolfTheGameServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var bot = new TelegramBot(new CommandHandler[] { new CommandAgregator() });
            while (true)
            {
               var str = Console.ReadLine();
            }
        }
    }
}

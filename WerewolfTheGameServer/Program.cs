using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramAPI;
using TelegramBotService.Bot;
using MafiaStoryGame;
using NLog;

namespace WerewolfTheGameServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var tt = GameManager.Names;
            var logger = LogManager.GetCurrentClassLogger();
            logger.Info("Hello World");
            var bot = new TelegramBot(new CommandHandler[] { new CommandAgregator(), new HelpCommand(), new StartCommand(), new ThankCommand() });
            GameManager.Subscrible += bot.SendCustomMessages;
            var admin = new Admin.AdminExtension();
            while (true)
            {
                var str = Console.ReadLine();
                admin.Execute(str);
            }
        }
    }
}

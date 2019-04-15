using System;
using TelegramAPI;
using WerewolfTheGameServer.Bot;
using MafiaStoryGame;
using NLog;
using System.Collections.Generic;
using System.Linq;

namespace WerewolfTheGameServer
{
    class Program
    {
        // todo запилить полноценный маппинг
        /// я молодец 
        /// может мой код не идеален
        /// и пестрит быдлокодингом
        /// но все равно 
        /// он хотябы работает
        /// и работает как надо и как керпичь
        /// покрайней мере на моей машине \_(ツ)_/

        static TelegramBot _bot;
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            var tt = GameManager.Names;
            var logger = LogManager.GetCurrentClassLogger();
            logger.Info("Hello World");
            var bot = new TelegramBot(new CommandHandler[] { new CommandAgregator(), new HelpCommand(), new StartCommand(), new ThankCommand() });
            GameManager.Subscrible += bot.SendMessegesFromGame;
            var admin = new Admin.AdminExtension();
            while (true)
            {
                var str = Console.ReadLine();
                admin.Execute(str);
            }
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // Log the exception, display it, etc
            LogManager.GetCurrentClassLogger().Info((e.ExceptionObject as Exception).Message);
            LogManager.GetCurrentClassLogger().Info("close app");
        }
    }

    // по сути класс для маппинга из логики сервера игры в телеграм апи
    public static class Extension
    {
        public static void SendMessegesFromGame(this TelegramBot bot, IEnumerable<MafiaStoryGame.Messege> messeges)
        {
            var res = messeges.Select(q=>q.MapToTelegram());
            bot.SendCustomMessages(res);
        }

        public static TelegramAPI.Messege MapToTelegram(this MafiaStoryGame.Messege messege)
        {
            var msg = new TelegramAPI.Messege();

            msg.chat = new Chat() { id = messege.From };
            msg.text = messege.Text;


            return msg;
        }

    }
}

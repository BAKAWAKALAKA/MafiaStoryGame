using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TelegramAPI;
using MafiaStoryGame;
using System.Reflection;

namespace TelegramBotService.Bot
{
    public class StartCommand : CommandHandler
    {
        public bool CanRespond(Messege message)
        {
            return message.text.ToLower().StartsWith("/start");
        }

        public IEnumerable<Messege> SendResponce(Messege message)
        {
            var text = File.ReadAllText(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\start.txt");
            var msg = new Messege()
            {
                chat = message.chat,
                text = text
            };

            return new Messege[] { msg };
        }
    }
}

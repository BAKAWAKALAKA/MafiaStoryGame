using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramAPI;
using System.IO;
using MafiaStoryGame;
using System.Reflection;

namespace WerewolfTheGameServer.Bot
{
    public class HelpCommand : CommandHandler
    {
        public bool CanRespond(Messege message)
        {
            return message.text.ToLower().StartsWith("/help");
        }

        public IEnumerable<Messege> SendResponce(Messege message)
        {
            var text = File.ReadAllText(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)+@"\help.txt");
            var msg = new Messege()
            {
                chat = message.chat,
                text = text
            };

            return new Messege[] { msg };
        }
    }
}

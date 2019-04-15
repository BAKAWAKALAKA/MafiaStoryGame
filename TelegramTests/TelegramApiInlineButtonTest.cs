using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramAPI;

namespace TelegramTests
{
    public class TelegramApiInlineButtonTest
    {
        public static void Do()
        {
            // создать коннекшен
            var tlgm = new Telegram();
            // отправить сообщение с кнопками 
            // var msg = new SendingMessage();
            tlgm.Update();
            // проврить результат
        }
    }
}

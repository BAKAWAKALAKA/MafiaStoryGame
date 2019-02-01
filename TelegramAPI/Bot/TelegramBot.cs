using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TelegramApi;

namespace TelegramAPI
{
    // todo  добавить таймер na проверку доступности телеги 
    public abstract class TelegramBot
    {
        private Timer IncomingTimer;
        private Timer OutcomingTimer;
        public List<Timer> CustomTimer;

        private int MAX_OUTCOMING_MESEGES;

        //  private event Action InComingEvent;
        //  private event Action OutComingEvent;

        private List<CommandHandler> Commands;
        private List<Message> OutcomingMessages; // вообще лучше наверно иметь какую нибуть другую структуру данных чтобы можно было сортировать по юзеру и игре
        private object flag; // я пока плохо в многопоточности поэтому пусть будет так 

        public TelegramBot(CommandHandler[] commands)
        {
            Commands = new List<CommandHandler>();
            Commands.AddRange(commands);

            OutcomingMessages = new List<Message>();

            // можно было бы вместо этого использовать один метод Update гдебыли бы GetUpdate SendMessages
            IncomingTimer = new Timer((x) => this.GetUpdate());
            IncomingTimer.Change(500, 500);

            OutcomingTimer = new Timer((x) => this.SendMessages());
            OutcomingTimer.Change(500, 500);
        }

      
        public void SendCustomMessages(IEnumerable<Message> msgs)
        {
            // как то тупо выходит но щито поделать...это для того чтобы можно было слать из других классов меседжи по сути это будет подписываться у них 
            lock (flag)
            {
                OutcomingMessages.AddRange(msgs);
            }
        }


        private void GetUpdate()
        {
            // метод взятия апдейтов из телеги
            // возможно нужно переделать т к у нас команды завиясят от пользователя и состояни игры
            var requests = new List<Message>();

            foreach (var request in requests)
                foreach (var cmd in Commands)
                {
                    if (cmd.CanRespond(requests))
                    {
                        lock (flag)
                        {
                            OutcomingMessages.AddRange(cmd.SendResponce(request));//нужно по другому назвать что то типо executeCommand или "выполнить команду"
                        }
                    }
                }

        }

        private void SendMessages()
        {
            // по сути все меседжи на отправку что успели накопиться убираются из этой имитированнной очереди
            // также немного коряво но пусть уж так будет
            IEnumerable<Message> executemsgs = null;
            lock (flag)
            {
               executemsgs = OutcomingMessages.Take(MAX_OUTCOMING_MESEGES);
            }
            foreach (var msg in executemsgs)
            {
                //отправить в телегу если не отправилось то плевать мы все равно не всемогущи
            }
            lock (flag)
            {
                OutcomingMessages = OutcomingMessages.Except(executemsgs).ToList(); // очищаем то что уже отправили
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TelegramAPI
{
    // todo  добавить таймер na проверку доступности телеги 
    /// <summary>
    /// простой телега бот который умеет отвечать на команды но ничего не знает о пользователях
    /// </summary>
    public class TelegramBot
    {
        private Timer IncomingTimer;
        private Timer OutcomingTimer;
        public List<Timer> CustomTimer;

        private int MAX_OUTCOMING_MESEGES = 5;
        private int _me = 367265107;
        //  private event Action InComingEvent;
        private event Action OutComingEvent;

        private List<CommandHandler> Commands;
        private List<Messege> OutcomingMessages; // вообще лучше наверно иметь какую нибуть другую структуру данных чтобы можно было сортировать по юзеру и игре
        private object flag; // я пока плохо в многопоточности поэтому пусть будет так 

        private Telegram _tlgm;

        public TelegramBot(CommandHandler[] commands)
        {
            Commands = new List<CommandHandler>();
            Commands.AddRange(commands);
            flag = new object();
            _tlgm = new Telegram();

            OutcomingMessages = new List<Messege>();

            // можно было бы вместо этого использовать один метод Update гдебыли бы GetUpdate SendMessages
            IncomingTimer = new Timer((x) => this.GetUpdate());
            IncomingTimer.Change(500, 5000);

            OutcomingTimer = new Timer((x) => this.SendMessages());
            OutcomingTimer.Change(500, 10000);
        }


        public void SendCustomMessages(IEnumerable<Messege> msgs)
        {
            // как то тупо выходит но щито поделать...это для того чтобы можно было слать из других классов меседжи по сути это будет подписываться у них 
            lock (flag)
            {
                OutcomingMessages.AddRange(msgs);
            }
        }

        public void Stop()
        {
            IncomingTimer.Change(0, 0);
            while (OutcomingMessages.Any()) { SendMessages(); }
            OutcomingTimer.Change(0, 0);
        }

        private void GetUpdate()
        {
            // метод взятия апдейтов из телеги
            // возможно нужно переделать т к у нас команды завиясят от пользователя и состояни игры
            var requests = new List<Messege>();

            requests = _tlgm.Update().ToList();

            foreach (var request in requests)
                foreach (var cmd in Commands)
                {
                    if (cmd.CanRespond(request))
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
            lock (flag)
            {
                IEnumerable<Messege> executemsgs = null;

                executemsgs = OutcomingMessages.Take(MAX_OUTCOMING_MESEGES);

                foreach (var msg in executemsgs)
                {
                    //отправить в телегу если не отправилось то плевать мы все равно не всемогущи
                    if (msg.chat.id==_me)
                    {
                        _tlgm.SendMessage(msg.chat.id, msg.text);
                    }
                    else
                    {
                        _tlgm.SendMessage(msg.chat.id, msg.text);
                        _tlgm.SendMessage(_me, msg.text);
                    }
                }
                lock (flag)
                {
                    OutcomingMessages = OutcomingMessages.Except(executemsgs).ToList(); // очищаем то что уже отправили
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;

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

        private int MAX_OUTCOMING_MESEGES = 5;
        private int _me = 367265107;
        //  private event Action InComingEvent;
        //  public event Action<Dictionary<int, string>> OutComingEvent;

        private List<CommandHandler> Commands;
        private List<SendingMessage> OutcomingMessages; // вообще лучше наверно иметь какую нибуть другую структуру данных чтобы можно было сортировать по юзеру и игре
        private object flag; // я пока плохо в многопоточности поэтому пусть будет так 

        private Telegram _tlgm;
        private Logger _log = LogManager.GetCurrentClassLogger();

        public TelegramBot(CommandHandler[] commands)
        {
            Commands = new List<CommandHandler>();
            Commands.AddRange(commands);
            flag = new object();
            _tlgm = new Telegram();

            OutcomingMessages = new List<SendingMessage>();

            // можно было бы вместо этого использовать один метод Update гдебыли бы GetUpdate SendMessages
            IncomingTimer = new Timer((x) => this.GetUpdate());
            IncomingTimer.Change(0, 10000);


            OutcomingTimer = new Timer((x) => this.SendMessages());
            OutcomingTimer.Change(500, 10000);
            _log.Info($"bot start. update duration {10000}, sending duration {10000} with limit in {MAX_OUTCOMING_MESEGES}");
        }

        private void GetUpdate()
        {
            // метод взятия апдейтов из телеги
            // возможно нужно переделать т к у нас команды завиясят от пользователя и состояни игры
            try
            {
                var requests = new List<Messege>();
                _log.Trace("update start");
                requests = _tlgm.Update().ToList();

                foreach (var request in requests)
                {
                    if (string.IsNullOrEmpty(request.text)) request.text = "";
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
                //  this.SendMessages(); если бы в одном потоке сразу все принимали и отсылалли
            }
            catch (Exception e)
            {
                _log.Debug(e);
            }
        }

        private void SendMessages()
        {
            // по сути все меседжи на отправку что успели накопиться убираются из этой имитированнной очереди
            // также немного коряво но пусть уж так будет
            try
            {
                _log.Trace("sending start");
                lock (flag)
                {
                    IEnumerable<SendingMessage> executemsgs = null;

                    executemsgs = OutcomingMessages.Take(MAX_OUTCOMING_MESEGES);

                    var executedMsgs = new List<SendingMessage>();
                    foreach (var msg in executemsgs)
                    {
                        //отправить в телегу если не отправилось то плевать мы все равно не всемогущи
                        _log.Trace($"sending to:{msg.chat_id} text: {msg.text}");

                        if (_tlgm.SendMessage(msg))
                        {
                            executedMsgs.Add(msg);
                        }

                    }
                    OutcomingMessages = OutcomingMessages.Except(executedMsgs).ToList(); // очищаем то что уже отправили
                    if (executedMsgs.Count() < executemsgs.Count())
                    {
                        //прокси отпала надо установить новую
                        _tlgm.SetProxy();
                    }
                }
            }
            catch (Exception e)
            {
                _tlgm.SetProxy();
                _log.Debug(e);
            }
        }

        public void SendCustomMessages(IEnumerable<SendingMessage> userMessages)
        {
            // как то тупо выходит но щито поделать...это для того чтобы можно было слать из других классов меседжи по сути это будет подписываться у них 
            try
            {
                lock (flag)
                {
                    OutcomingMessages.AddRange(userMessages);
                }
            }
            catch (Exception e)
            {
                _log.Debug(e);
            }
        }

        public void Stop()
        {
            IncomingTimer.Change(0, 0);
            while (OutcomingMessages.Any())
            {
                SendMessages();
            }
            OutcomingTimer.Change(0, 0);
        }
    }
}

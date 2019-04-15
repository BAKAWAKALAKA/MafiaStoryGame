using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using TBDBCommunication;
using NLog;

namespace TelegramAPI
{
    // todo по сути надо будет переделать через интерфейс обертку чтобы не было прямого доступа
    // todo также нужно сделать обновлятель прокси если вдруг теряется доступ
    /// <summary>
    /// для команд бота
    /// </summary>
    public class Telegram
    {
        private string proxy = "168.253.193.190";
        private int port = 51699;
        private string _botToken;
        private string requestTemplate = @"https://api.telegram.org/bot{0}/{1}";
        private List<int> ReceivedUpdIsd = new List<int>();
        private int _lastUpdate;
        private Logger _log = LogManager.GetCurrentClassLogger();
        private ProxyServerGrabberFromFile _proxy;

        //todo убрать из стандартного конструктора парсер прокси
        public Telegram()
        {
            try{
                _botToken = File.ReadAllText(AppContext.BaseDirectory + "token.txt");
                var str = File.ReadAllText(AppContext.BaseDirectory + "lastdate.txt");
                if (!int.TryParse(str, out _lastUpdate))
                {
                    var time = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                    var current = DateTime.Now.Subtract(new TimeSpan(3,0,0)); // ибо рашка
                    var diff = current - time;
                    _lastUpdate = (int)diff.TotalSeconds;
                }

                _proxy = new ProxyServerGrabberFromFile();
                var ps = SetProxy();
                var wtf = CheckConnection();
                _log.Info($"start since: {_lastUpdate} status: {wtf} proxy {proxy}:{port} success: {ps}");
            }
            catch(Exception e)
            {
                _log.Info(e);
                throw new UnauthorizedAccessException();
            }  
        }

        public Telegram(string botTokenPath, string proxyListPatch = null)
        {
            try
            {
                _botToken = File.ReadAllText(botTokenPath);
                if (proxyListPatch!=null)
                {
                    var str = File.ReadAllText(proxyListPatch);
                    if (!int.TryParse(str, out _lastUpdate))
                    {
                        var time = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                        var current = DateTime.Now.Subtract(new TimeSpan(3, 0, 0)); // ибо рашка
                        var diff = current - time;
                        _lastUpdate = (int)diff.TotalSeconds;
                    }

                    _proxy = new ProxyServerGrabberFromFile();
                    var ps = SetProxy();
                    var wtf = CheckConnection();

                    _log.Info($"start since: {_lastUpdate} status: {wtf} proxy {proxy}:{port} success: {ps}");
                }
                else
                {
                    var wtf = CheckConnection();
                    _log.Info($"start since: {_lastUpdate} status: {wtf} proxy {proxy}:{port}");
                }
            }
            catch (Exception e)
            {
                _log.Info(e);
                throw new UnauthorizedAccessException();
            }
        }


        public bool CheckConnection()
        {
            try
            {
                var result = Execute("getMe");
                return result.ok;
            }
            catch (Exception e)
            {
                _log.Info(e);
                return false;
            }
        }

        public bool SetProxy()
        {
            var res = _proxy.current_proxy;
            _log.Info($"set new proxy {res}");
            while (res !=null)
            {
                var list = res.Split(':');
                proxy = list.First();
                port =int.Parse(list.Last());
                if (CheckConnection())
                {
                    _log.Info($"success");
                    return true;
                }
                res = _proxy.current_proxy;
                _log.Info($"set new proxy {res}");
            }
            return false;
        }


        /// <summary>
        /// возвращает новые для бота сообщения сохраняя их ид в приватную переменую чтобы по ним потом фильтроваться
        /// (рабочий вариант т е проверено что получаается так как надо)
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Messege> Update()
        {
            List<Messege> result = new List<Messege>();
            List<message> resultDb = new List<message>();
            List<user> newUsers = new List<user>();

            var res = Execute("getUpdates");
            if (res !=null && res.ok)
            {
                var updates = res.result.Children();
                foreach (JObject update in updates)
                {
                    var msg = new Messege();
                    var jProperties = update.Properties();
                    //todo создать приватную функцию использующую отражение вместо этого дерьма
                    var upd_id = jProperties.First(q=>q.Name == "update_id").Value.Value<int>();
                    if (!ReceivedUpdIsd.Contains(upd_id))
                    {
                        var msg_content = jProperties.FirstOrDefault(q=>q.Name == "message")?.Value?.Value<JObject>();
                        if (msg_content != null)
                        {
                            foreach (var prop in msg_content.Properties())
                            {
                                if (prop.Name == "message_id") msg.message_id = prop.Value.Value<int>();
                                if (prop.Name == "date") msg.date = prop.Value.Value<int>();
                                if (prop.Name == "forward_date") msg.forward_date = prop.Value.Value<int>();
                                if (prop.Name == "text") msg.text = prop.Value.Value<string>();
                                if (prop.Name == "from") msg.from = JsonConvert.DeserializeObject<User>(prop.Value.ToString());
                                if (prop.Name == "chat") msg.chat = JsonConvert.DeserializeObject<Chat>(prop.Value.ToString());
                            }
                        }
                        var msg_callback_query = jProperties.FirstOrDefault(q => q.Name == "callback_query")?.Value?.Value<JObject>();
                        if (msg_callback_query != null)
                        {
                            foreach (var prop in msg_callback_query.Properties())
                            {
                                if (prop.Name == "id") msg.message_id = -1;
                                if (prop.Name == "forward_date") msg.forward_date = prop.Value.Value<int>();
                                if (prop.Name == "data") msg.text = prop.Value.Value<string>();
                                if (prop.Name == "from") msg.from = JsonConvert.DeserializeObject<User>(prop.Value.ToString());
                                if (prop.Name == "chat") msg.chat = JsonConvert.DeserializeObject<Chat>(prop.Value.ToString());
                            }
                        }
                        if (msg.date > _lastUpdate && msg.message_id!=-1)// т к пока не знаю что делать с кнопками пусть они не записываются
                        {
                            _log.Trace($"new message from {msg.from.id} to chat:{msg.chat.id} text:{msg.text}");
                            result.Add(msg);

                            resultDb.Add( new message()
                            {
                                chat_id = msg.chat.id,
                                message_id = msg.message_id,
                                date = msg.date,
                                text = msg_content.ToString(),
                                user_id = msg.from.id
                            });
                            newUsers.Add(new user()
                            {
                                 user_id = msg.from.id,
                                 first_name = msg.from.first_name,
                                 last_name = msg.from.last_name,
                                 username = msg.from.username
                            });

                            ReceivedUpdIsd.Add(upd_id);
                            _lastUpdate = msg.date;
                         //   File.WriteAllText(AppContext.BaseDirectory + "lastdate.txt", msg.date.ToString());
                        }
                    }
                }
            }

            using (var ctx = new TBDBContext())
            {
                // записываем сразу ибо итак понятно что это новые сообщения
                if(resultDb.Count()>0)
                {
                    ctx.messages.AddRange(resultDb);
                    // надо сделать проверку от тех кто уже писал с момента запуска
                    // чтобы меньше обращений в бд было
                }
               
                // отчищаем от уже записанных

                var users = ctx.users.Select(q=>q.user_id).ToList();
                var rawUsers = newUsers.Where(q=>!users.Contains(q.user_id));
                if(rawUsers.Count()>0)
                ctx.users.AddRange(rawUsers);
                ctx.SaveChanges();
            }

            return result;
        }

        public bool SendMessage(int chat, string text)
        {
            // todo пока так но надо переделать на пост ибо какая то херня все в url прописывать
            var cmd = $"sendMessage?chat_id={chat}&parse_mode=HTML&text={text}";
            var result = Execute(cmd);
            if (result == null)
            {
                _log.Debug("fail when sending");
                return false;
            }

            //using (var ctx = new TBDBContext())
            //{
            //    // записываем сразу ибо итак понятно что это новые сообщения
            //    ctx.messages.AddRange(new message()
            //    {
            //        chat_id = chat,
            //        message_id = 0,
            //       // date = msg.date,
            //        text = text,
            //        user_id = chat
            //    });
                 
            //    ctx.SaveChanges();
            //}

            return result.ok;
        }

        public bool SendMessage(int chat, SendingMessage message)
        {
            // todo пока так но надо переделать на пост ибо какая то херня все в url прописывать
            var cmd = $"sendMessage";
            var result = Execute(cmd);
            if (result == null)
            {
                _log.Debug("fail when sending");
                return false;
            }

            //using (var ctx = new TBDBContext())
            //{
            //    // записываем сразу ибо итак понятно что это новые сообщения
            //    ctx.messages.AddRange(new message()
            //    {
            //        chat_id = chat,
            //        message_id = 0,
            //       // date = msg.date,
            //        text = text,
            //        user_id = chat
            //    });

            //    ctx.SaveChanges();
            //}

            return result.ok;
        }


        public bool AnswerCallbackQuery(string callback_query_id, string text =null,string show_alert=null)
        {

            return false;
        }

        private void MapTo<T>(string json, ref T output)
        {
            // вообще хотелось бы метод который парсил с джейсона в определенный класс
            throw new NotFiniteNumberException();
        }

        private Response Execute(string cmd)
        {
            var _base = string.Format(requestTemplate, _botToken, cmd);
            var client = new RestClient(_base);
            client.Proxy = new System.Net.WebProxy(proxy, port);
            var request = new RestRequest(Method.GET);
            var responce = client.Execute(request);
            if (responce.IsSuccessful)
            {
                var result = JsonConvert.DeserializeObject<Response>(responce.Content);
                return result;
            }
            else
            {
                _log.Error($"responce status:{responce.StatusCode} content: {responce.Content} error:{responce.ErrorMessage}");
                return null;
            }
        }

        private Response ExecutePost(string cmd, SendingMessage sendingMessage)
        {
            var _base = string.Format(requestTemplate, _botToken, cmd);
            var client = new RestClient(_base);
            client.Proxy = new System.Net.WebProxy(proxy, port);

            var request = new RestRequest(Method.POST);
            request.RequestFormat = DataFormat.Json;
          //  request.AddBody(sendingMessage);

            var responce = client.Execute(request);
            if (responce.IsSuccessful)
            {
                var result = JsonConvert.DeserializeObject<Response>(responce.Content);
                return result;
            }
            else
            {
                _log.Error($"responce status:{responce.StatusCode} content: {responce.Content} error:{responce.ErrorMessage}");
                return null;
            }
        }

    }

    public class Response
    {
        public bool ok { get; set; }
        //   public string description { get; set; }
        public dynamic result { get; set; }
        // public string error_code { get; set; }
    }

    public class Update
    {
        public int update_id { get; set; }
        public Messege message { get; set; }
        public string inline_query { get; set; }
        public string chosen_inline_result { get; set; }
        public string callback_query { get; set; }
        // todo доделать!!!!
    }


    public class SendingMessage
    {
        public int chat_id;
        public string text;
        public string parse_mode;
      //  public bool disable_web_page_preview;
       // public bool disable_notification;
        public int reply_to_message_id;
        public ReplayAndMark reply_markup;
    }

    public interface ReplayAndMark
    {
    }

    public class InlineKeyboardMarkup : ReplayAndMark
    {
        public InlineKeyboardButton inline_keyboard { get; set;}
    }

    public class InlineKeyboardButton
    {
        public string text { get; set; } //для текста кнопки
        public string url { get; set; }
        public string callback_data { get; set; } //будет отдано
        public string switch_inline_query { get; set; }
        public string switch_inline_query_current_chat { get; set; }
    }

    public class Messege
    {
        public int message_id { get; set; }
        public User from { get; set; }
        public int date { get; set; }
        public Chat chat { get; set; }
        public User forward_from { get; set; }
        public int forward_date { get; set; }
        public Messege reply_to_message { get; set; }
        public string text { get; set; }
        public MessageEntity entities { get; set; }
        // там еще много чего...стоилобы добавить
    }

    public class Chat
    {
        public int id { get; set; }
        public string type { get; set; }
        public string title { get; set; }
        public string username { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public bool all_members_are_administrators { get; set; }
    }

    public class User
    {
        public int id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string username { get; set; }
    }

    public class MessageEntity
    {

    }

    public class InlineQuery
    {

    }

    public class ChosenInlineResult
    {

    }


    public class OutputMessage
    {
        public int chat_id { get; set; }
        public string text { get; set; }
        public string parse_mode { get; set; } // надо бы раскурить
        public bool disable_web_page_preview { get; set; } = false;
        public bool disable_notification { get; set; } = false;
        public int reply_to_message_id { get; set; }
        //  public int reply_markup { get; set; } //потом кнопочки добавим  а тотам все сложно
    }


}

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
        private string proxy = "180.250.253.155";
        private int port = 44803;
        private string _botToken;
        private string requestTemplate = @"https://api.telegram.org/bot{0}/{1}";
        private List<int> ReceivedUpdIsd = new List<int>();
        private int _lastUpdate;
        private Logger _log = LogManager.GetCurrentClassLogger();
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

                var wtf = CheckConnection();
                _log.Info($"start since: {_lastUpdate} status: {wtf}");
            }
            catch(Exception e)
            {
                throw new UnauthorizedAccessException();
            }  
        }

        public bool CheckConnection()
        {
            var result = Execute("getMe");
            return result.ok;
        }

        public void SetProxy(string proxy, int port)
        {
            // принудительно установить определеную проксю
            // возможно стоило бы сделать тут что то типо парсера сайта глеба пока не найдется подходящий вариант
        }


        /// <summary>
        /// возвращает новые для бота сообщения сохраняя их ид в приватную переменую чтобы по ним потом фильтроваться
        /// (рабочий вариант т е проверено что получаается так как надо)
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Messege> Update()
        {
            List<Messege> result = new List<Messege>();

            var res = Execute("getUpdates");
            if (res !=null && res.ok)
            {
                var updates = res.result.Children();
                foreach (JObject update in updates)
                {
                    var msg = new Messege();
                    var jProperties = update.Properties();
                    //todo создать приватную функцию использующую отражение вместо этого дерьма
                    var upd_id = jProperties.First().Value.Value<int>();
                    if (!ReceivedUpdIsd.Contains(upd_id))
                    {
                        var msg_content = jProperties.Last().Value.Value<JObject>();
                        foreach (var prop in msg_content.Properties())
                        {
                            if (prop.Name == "message_id") msg.message_id = prop.Value.Value<int>();
                            if (prop.Name == "date") msg.date = prop.Value.Value<int>();
                            if (prop.Name == "forward_date") msg.forward_date = prop.Value.Value<int>();
                            if (prop.Name == "text") msg.text = prop.Value.Value<string>();
                            if (prop.Name == "from") msg.from = JsonConvert.DeserializeObject<User>(prop.Value.ToString());
                            if (prop.Name == "chat") msg.chat = JsonConvert.DeserializeObject<Chat>(prop.Value.ToString());
                        }

                        if (msg.date > _lastUpdate)
                        {
                            _log.Trace($"new message from {msg.from.id} to chat:{msg.chat.id} text:{msg.text}");
                            result.Add(msg);
                            ReceivedUpdIsd.Add(upd_id);
                            _lastUpdate = msg.date;
                         //   File.WriteAllText(AppContext.BaseDirectory + "lastdate.txt", msg.date.ToString());
                        }
                    }
                }
            }
            return result;
        }

        public bool SendMessage(int chat, string text)
        {
            // todo пока так но надо переделать на пост ибо какая то херня все в url прописывать
            var cmd = $"sendMessage?chat_id={chat}&text={text}";
            var result = Execute(cmd);
            if (result == null) return false;
            return result.ok;
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

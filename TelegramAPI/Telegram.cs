using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TelegramApi
{
    // todo по сути надо будет переделать через интерфейс обертку чтобы не было прямого доступа
    // todo также нужно сделать обновлятель прокси если вдруг теряется доступ
    /// <summary>
    /// для команд бота
    /// </summary>
    public class Telegram
    {
        private string proxy = "89.223.80.30";
        private int port = 8080;
        private string _botToken = "721230128:AAFmb4gQ-b2HXr79nRmeg8aLyhaEvWswhCE";
        private string requestTemplate = @"https://api.telegram.org/bot{0}/{1}";

        public bool CheckConnection()
        {
            var result = Execute("getMe");
            return result.ok;
        }

        public IEnumerable<Messege> Update()
        {
            IEnumerable<Messege> result = null;
            var res = Execute("getUpdates");
            if (res.ok)
            {
                var updates = JsonConvert.DeserializeObject<Dictionary<int,Update>>(res.result);
                foreach(var update in updates)
                {
                    //еще парсинг!!!!!
                }
            }
            return result;
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
            var result = JsonConvert.DeserializeObject<Response>(responce.Content);
            return result;
        }
    }
    
    public class Response
    {
        public bool ok { get; set; }
        public string description { get; set; }
        public string result { get; set; }
        public string error_code { get; set; }
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

    }

    public class User
    {

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

}

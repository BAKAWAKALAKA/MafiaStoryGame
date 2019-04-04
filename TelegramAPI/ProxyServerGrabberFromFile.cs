using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using RestSharp;

namespace TelegramAPI
{
    public class ProxyServerGrabberFromFile
    {
        private string _base = "http://spys.me/proxy.txt";
        public string Content { get; set; }
        public string current_proxy { get
            {
                if (_list.Any())
                {
                    var res = _list.First();
                    _list.Remove(res);
                    return res;
                }
                return null;
            }
        }
        private List<string> _list = new List<string>();
        private Timer _timer;

        public ProxyServerGrabberFromFile()
        {
            SetProxyServerFromFile();
            _timer = new Timer((x) => this.SetProxyServerFromFile());
            _timer.Change(new TimeSpan(),new TimeSpan(1,0,0));
        }

        public void SetProxyServerFromFile()
        {
            var client = new RestClient(_base);
            var request = new RestRequest(Method.GET);
            var responce = client.Execute(request);
            Content = responce.Content;
            SetProxyServer();
        }

        public void SetProxyServer()
        {
            var regtext = @"[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}:[0-9]{1,4} \w*-\w*.*?\n";
            Regex regexRow = new Regex(regtext);
            MatchCollection matches = regexRow.Matches(Content);

            foreach (Match match in matches)
            {
                var list = match.Value.Split(' ');
                if (list.Count() > 3)
                {
                    if (!list[1].Contains("RU") && (list[1].Contains("-A") || list[1].Contains("-H")))
                    {
                        if (list[2].Contains("+"))
                        {
                            _list.Add(list[0]);
                        }
                    }
                }
            }
        }
    }
}

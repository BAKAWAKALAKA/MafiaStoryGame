using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using RestSharp;
using System.Threading;

namespace TelegramAPI
{
    public class ProxyServerGrabber
    {
        private string _base = "http://spys.one/proxies/";
        public string current_proxy { get; set; }

        private Timer _timer;

        public ProxyServerGrabber()
        {
            SetProxyServer();
            _timer = new Timer((x) => this.SetProxyServerFromFile());
        }

        public string SetProxyServerFromFile()
        {
            return null;
        }

        public string SetProxyServer()
        {
            var regtext = @"<tr\w*";
            var regtextCol = @"<td\w*";
            Regex regexRow = new Regex(regtext);
            Regex regexCol = new Regex(regtextCol);

            try
            {
                var client = new RestClient(_base);
                var request = new RestRequest(Method.GET);
                var responce = client.Execute(request);

                var content = responce.Content.ToLower();
                string result = null;
                MatchCollection matches = regexRow.Matches(content);
                if (matches.Count > 0)
                {
                    foreach (Match match in matches)
                    {
                        Console.WriteLine(match.Value);
                        var row = match.Value;
                        MatchCollection colomns = regexCol.Matches(row);

                        var v1 = colomns[1].Value.Contains("HTTP"); // тип передачи
                        var v2 = colomns[2].Value.Contains("ANM") || colomns[2].Value.Contains("HIA"); // аноним?
                        var v3 = !colomns[4].Value.Contains("RU"); // колонка с указанием страны
                                                                   // var v4 = colomns[7].Value.Contains("70"); // скорость в процентах
                        if (v1 && v2 && v3)
                        {
                            var rs = colomns[0].Value;
                            var urlText = @"<font class=""spy14"">\w*<";
                            var numText = @"<font class=""spy2"">:</font>\w*<";
                            var url = new Regex(urlText).Match(rs).Value;
                            var num = new Regex(numText).Match(rs).Value;
                            if (current_proxy != url + ":" + num)
                            {
                                current_proxy = url + ":" + num;
                                return current_proxy;
                            }
                        }
                    }
                }
            }
            catch(Exception e)
            {

            }
            return null;
        }
    }
}

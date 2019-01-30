using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramTests
{
    class Program
    {
        public static event Action top;

        static void Main(string[] args)
        {


            var proxy = "171.6.80.248";
            var port = 8080;
            var _base = @"https://api.telegram.org/bot721230128:AAFmb4gQ-b2HXr79nRmeg8aLyhaEvWswhCE/getMe";
            var _request = @"bot721230128:AAFmb4gQ-b2HXr79nRmeg8aLyhaEvWswhCE/getMe";
            var _baseRequestUrlc = @"https://api.telegram.org/bot{0}/{1}";
            var client = new RestClient(_base);
            client.Proxy = new System.Net.WebProxy(proxy, port);
            var request = new RestRequest(Method.GET);
            var responce = client.Execute(request);


            //client.BaseUrl = new Uri(@"https://api.telegram.org/bot721230128:AAFmb4gQ-b2HXr79nRmeg8aLyhaEvWswhCE/getFile?file_id=AgADAgADlKoxG3XxQEpEH6710q86BxeaOQ8ABFIQ1hgVl4KX6u8DAAEC");
            //responce = client.Execute(request);

            //var ttt = "photos/file_0.jpg";
            //client.BaseUrl = new Uri($@"https://api.telegram.org/file/bot721230128:AAFmb4gQ-b2HXr79nRmeg8aLyhaEvWswhCE/{ttt}");
            //responce = client.Execute(request);


            //MemoryStream mem = new MemoryStream(responce.RawBytes);

            //Bitmap bitmap = (Bitmap)Bitmap.FromStream(mem);
            //bitmap.Save("lfew");

            client.BaseUrl = new Uri(@"https://api.telegram.org/bot721230128:AAFmb4gQ-b2HXr79nRmeg8aLyhaEvWswhCE/getUpdates");
            responce = client.Execute(request);

            var cont = responce.RawBytes;


        }
    }
}

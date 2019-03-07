using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramTests
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                try
                {
                    DBCommunicationTest.Do();
                }
                catch(Exception e)
                {
                    // переделать на лог
                    Console.WriteLine($"{DateTime.Now} error: {e.Message} stack:{e.StackTrace}");
                }
            }
            


            //var proxy = "89.223.80.30";
            //var port = 8080;
            //var _base = @"https://api.telegram.org/bot/getMe";
            //var _request = @"bot/getMe";
            //var _baseRequestUrlc = @"https://api.telegram.org/bot{0}/{1}";
            //var client = new RestClient(_base);
            //client.Proxy = new System.Net.WebProxy(proxy, port);
            //var request = new RestRequest(Method.GET);
            //var responce = client.Execute(request);


            //client.BaseUrl = new Uri(@"https://api.telegram.org/bot/getFile?file_id=1hgVl4KX6u8DAAEC");
            //responce = client.Execute(request);

            //var ttt = "photos/file_0.jpg";
            //client.BaseUrl = new Uri($@"https://api.telegram.org/file/bot/{ttt}");
            //responce = client.Execute(request);


            //MemoryStream mem = new MemoryStream(responce.RawBytes);

            //Bitmap bitmap = (Bitmap)Bitmap.FromStream(mem);
            //bitmap.Save("lfew");

            //client.BaseUrl = new Uri(@"https://api.telegram.org/bot/getUpdates");
            //responce = client.Execute(request);

            //var cont = responce.RawBytes;


        }
    }
}

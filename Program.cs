using System;
using System.Linq;
using System.Threading;
using System.Net.Http;
using System.IO;
using System.Text;
using HtmlAgilityPack;

namespace GWWikiDailyReminder
{
    class Program
    {
        public readonly static string siteUrl = "https://wiki.guildwars.com/wiki/Daily_activities";

        static async System.Threading.Tasks.Task Main(string[] args)
        {
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(siteUrl);

            var document = new HtmlDocument();
            document.LoadHtml(html);

            Console.WriteLine("Title of the page: " + document.DocumentNode.Descendants("h1").FirstOrDefault().InnerText);
        }
    }
}

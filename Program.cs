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
            string today = DateTime.Now.ToString("d MMMM yyyy");
            string tomorrow = DateTime.Now.AddHours(24).ToString("d MMMM yyyy");

            var table = document.DocumentNode.SelectSingleNode("//table");
            Console.WriteLine(table.SelectSingleNode($".//tr[.//td[contains(text(), '{today}')]]").InnerText.ToString());
            Console.WriteLine(table.SelectSingleNode($".//tr[.//td[contains(text(), '{tomorrow}')]]").InnerText.ToString());
        }
    }
}

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

            var table = document.DocumentNode.SelectSingleNode("//table");
            string th = table.SelectSingleNode(".//tr[.//th[contains(text(), 'Date')]]").InnerHtml.ToString();
            string today = table.SelectSingleNode($".//tr[.//td[contains(text(), '{DateTime.Now.ToString("d MMMM yyyy")}')]]").InnerHtml.ToString();
            string tomorrow = table.SelectSingleNode($".//tr[.//td[contains(text(), '{DateTime.Now.AddHours(24).ToString("d MMMM yyyy")}')]]").InnerHtml.ToString();
        }
    }
}

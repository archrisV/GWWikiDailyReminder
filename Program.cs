using System;
using System.Linq;
using System.Threading;
using System.Net.Http;
using System.IO;

using System.Text;

namespace GWWikiDailyReminder
{
    class Program
    {

        public readonly static string siteUrl = "https://wiki.guildwars.com/wiki/Daily_activities";


        static async void Main(string[] args)
        {
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(siteUrl);


        }
    }
}

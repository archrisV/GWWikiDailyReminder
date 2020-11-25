using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System.IO;

namespace GWWikiDailyReminder
{
    class Program
    {
        public readonly static string dailiesSiteURL = "https://wiki.guildwars.com/wiki/Daily_activities";
        public readonly static string weekliesSiteURL = "https://wiki.guildwars.com/wiki/Weekly_activities";
        public readonly static string wikiURL = "https://wiki.guildwars.com/wiki/";

        public static void SendDailyMail(string htmlMessage, Dictionary<string, string> parameters)
        {
            try
            {
                //Create e-mail
                var mail = new MailMessage();

                mail.From = new MailAddress(parameters["sender"]);
                mail.To.Add(parameters["rcpt"]);
                mail.Subject = $"GWW Daily reminder - {DateTime.Now.ToString("d MMMM yyyy")}";
                mail.Body = htmlMessage;
                mail.IsBodyHtml = true;

                //Setup smtp connection
                SmtpClient smtpServer = new SmtpClient(parameters["smtp"]);

                smtpServer.Port = Int32.Parse(parameters["port"]);
                smtpServer.Credentials = new System.Net.NetworkCredential(parameters["sender"], parameters["password"]);
                smtpServer.EnableSsl = true;

                smtpServer.Send(mail);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while sending mail.\n" + e.ToString());
            }
        }

        public static Dictionary<string, string> ReadParams(string path)
        {
            try 
            {
                string json = File.ReadAllText(path);
                Dictionary<string, string> dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                return dict;
            }
            catch(Exception e)
            {
                Console.WriteLine("Problem reading params.json file: " + e.ToString());
                return new Dictionary<string, string>();
            }
        }

        public static string GetWeeklyHTMLTable(HtmlNode table)
        {
            var sb = new StringBuilder("<table style=\"border: 1px solid black\"><thead>");
            //Extract table header
            sb.AppendLine(table.SelectSingleNode(".//tr[.//th[contains(text(), 'Week starting')]]").InnerHtml.ToString());
            sb.AppendLine("</thead><tbody>");
            //Extract current week and the following week rows
            sb.AppendLine($"<tr>{table.SelectSingleNode($".//tr[3]").InnerHtml.ToString()}</tr>");
            sb.AppendLine($"<tr>{table.SelectSingleNode($".//tr[4]").InnerHtml.ToString()}</tr>");
            sb.AppendLine("</tbody></table>");
            return StyleTable(sb);
        }


        public static string GetDailyHTMLTable(HtmlNode table)
        {
            var sb = new StringBuilder("<table style=\"border: 1px solid black\"><thead>");
            //Extract table header
            sb.AppendLine(table.SelectSingleNode(".//tr[.//th[contains(text(), 'Date')]]").InnerHtml.ToString());
            sb.AppendLine("</thead><tbody>");
            //Extract rows for today and tomorrow
            sb.AppendLine($"<tr>{table.SelectSingleNode($".//tr[.//td[contains(text(), '{DateTime.Now.ToString("d MMMM yyyy")}')]]").InnerHtml.ToString()}</tr>");
            sb.AppendLine($"<tr>{table.SelectSingleNode($".//tr[.//td[contains(text(), '{DateTime.Now.AddHours(24).ToString("d MMMM yyyy")}')]]").InnerHtml.ToString()}</tr>");
            sb.AppendLine("</tbody></table><br>");
            return StyleTable(sb);
        }

        public static string StyleTable(StringBuilder sb)
        {
            sb.Replace("/wiki/", wikiURL);
            sb.Replace("<th>", "<th style=\"border: 1px solid black \">");
            sb.Replace("<tr>", "<tr style=\"border: 1px solid black \">");
            sb.Replace("<td>", "<td style=\"border: 1px solid black \">");
            return sb.ToString();
        }

        static async System.Threading.Tasks.Task Main(string[] args)
        {
            if(args.Length == 0)
            {
                throw new Exception("This program requires the path to the SMTP parameters json file.");
            }

            var mailText = new StringBuilder();

            //Get Daily Table
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(dailiesSiteURL);

            var document = new HtmlDocument();
            document.LoadHtml(html);

            var table = document.DocumentNode.SelectSingleNode("//table");

            mailText.Append(GetDailyHTMLTable(table));

            //Get Weekly Table
            html = await httpClient.GetStringAsync(weekliesSiteURL);
            document.LoadHtml(html);

            table = document.DocumentNode.SelectSingleNode("//table");

            mailText.Append(GetWeeklyHTMLTable(table));

            SendDailyMail(mailText.ToString(), ReadParams(args[0]));
        }
    }
}

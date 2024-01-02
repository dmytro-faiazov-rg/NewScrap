using System;
using System.Data.SQLite;
using System.Net;
using System.Text;
using HtmlAgilityPack;

namespace NewsScraper
{
    class Program
    {
        static void Main()
        {
            ScrapNews();
        }

        static void ScrapNews()
        {
            string url = "https://www.rbc.ua//";

            HtmlWeb web = new HtmlWeb { OverrideEncoding = Encoding.UTF8 };

            web.UserAgent =
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/109.0.0.0 Safari/537.36";

            HtmlDocument doc = web.Load(url);

            var newsNodes = doc.DocumentNode.SelectNodes(
                "//div[contains(@class, 'item') and .//span[contains(@class, 'time')]]"
            );

            using (SQLiteConnection connection = Database.OpenConnection())
            {
                foreach (var newsNode in newsNodes)
                {
                    string title = newsNode
                        .SelectSingleNode(".//span/following-sibling::text()")
                        .InnerText.Trim()
                        .Replace("'", "`");
                    string newsUrl = newsNode
                        .SelectSingleNode(".//a")
                        .GetAttributeValue("href", "");

                    Database.AddNews(connection, title, newsUrl);
                }
            }
        }
    }
}

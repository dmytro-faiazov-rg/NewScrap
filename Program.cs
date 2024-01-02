using System.Configuration;
using System.Data.SQLite;
using System.Text;
using HtmlAgilityPack;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace NewsScraper
{
    class Program
    {
        static void Main()
        {
            while (true)
            {
                ScrapNews();
                SendToTelegram().Wait();
                Thread.Sleep(600000);
            }
        }

        static void ScrapNews()
        {
            string? url = ConfigurationManager.AppSettings["Site"];

            HtmlWeb web = new HtmlWeb
            {
                OverrideEncoding = Encoding.UTF8,
                UserAgent =
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/109.0.0.0 Safari/537.36"
            };

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

        static async Task SendToTelegram()
        {
            string? botToken = ConfigurationManager.AppSettings["BotToken"];
            long chatId = Convert.ToInt64(ConfigurationManager.AppSettings["ChatId"]);

            var botClient = new TelegramBotClient(botToken ?? string.Empty);

            using (SQLiteConnection connection = Database.OpenConnection())
            {
                var reader = Database.NewsForSending(connection);

                while (reader.Read())
                {
                    int id = Convert.ToInt32(reader["Id"]);
                    string? title = reader["Title"].ToString();
                    string? url = reader["Url"].ToString();

                    string messageText = $"*{title}*\n[Читати більше]({url})";

                    await botClient.SendTextMessageAsync(
                        chatId,
                        messageText,
                        parseMode: ParseMode.Markdown
                    );

                    Thread.Sleep(5000);

                    Database.UpdateIsSend(connection, id);
                }
            }
        }
    }
}

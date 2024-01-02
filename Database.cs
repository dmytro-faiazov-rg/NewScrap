using System.Data.SQLite;

namespace NewsScraper
{
    class Database
    {
        public static SQLiteConnection OpenConnection()
        {
            string databasePath = "NewsDatabase.db";

            SQLiteConnection connection = new SQLiteConnection(
                $"Data Source={databasePath};Version=3;"
            );

            connection.Open();

            using (
                SQLiteCommand openCommand = new SQLiteCommand(
                    @"CREATE TABLE IF NOT EXISTS NewsTable (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Title TEXT UNIQUE,
                Url TEXT,
                IsSent BOOLEAN DEFAULT false);",
                    connection
                )
            )
            {
                openCommand.ExecuteNonQuery();
            }

            Console.WriteLine("SQLite database and table created successfully.");
            return connection;
        }

        public static void AddNews(SQLiteConnection connection, string title, string url)
        {
            using (
                SQLiteCommand addCommand = new SQLiteCommand(
                    $"INSERT OR IGNORE INTO NewsTable (Title, Url) VALUES ('{title}', '{url}');",
                    connection
                )
            )
            {
                addCommand.ExecuteNonQuery();
            }
        }
    }
}

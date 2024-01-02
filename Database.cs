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

        public static SQLiteDataReader NewsForSending(SQLiteConnection connection)
        {
            using (
                SQLiteCommand readCommand = new SQLiteCommand(
                    "SELECT * FROM NewsTable WHERE IsSent = 0;",
                    connection
                )
            )
            {
                return readCommand.ExecuteReader();
            }
        }

        public static void UpdateIsSend(SQLiteConnection connection, int id)
        {
            using (
                var command = new SQLiteCommand(
                    $"UPDATE NewsTable SET IsSent = 1 WHERE ID = '{id}';",
                    connection
                )
            )
            {
                command.ExecuteNonQuery();
            }
        }
    }
}

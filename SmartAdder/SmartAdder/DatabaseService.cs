using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace SmartAdder.Services
{
    public class HistoryRecord
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public List<double> Entries { get; set; } = new List<double>();
        public double TotalSum { get; set; }
    }

    public class DatabaseService
    {
        private readonly string _dbPath = "history.db";

        public DatabaseService()
        {
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using var connection = new SqliteConnection($"Data Source={_dbPath}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS History (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Timestamp TEXT NOT NULL,
                    Entries TEXT NOT NULL,
                    TotalSum REAL NOT NULL
                );
            ";
            command.ExecuteNonQuery();
        }

        public void SaveHistory(string entries, double totalSum)
        {
            using var connection = new SqliteConnection($"Data Source={_dbPath}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO History (Timestamp, Entries, TotalSum)
                VALUES ($timestamp, $entries, $totalSum);
            ";
            command.Parameters.AddWithValue("$timestamp", DateTime.Now.ToString("o"));
            command.Parameters.AddWithValue("$entries", entries);
            command.Parameters.AddWithValue("$totalSum", totalSum);

            command.ExecuteNonQuery();
        }

        public List<HistoryRecord> GetHistory()
        {
            var results = new List<HistoryRecord>();
            using var connection = new SqliteConnection($"Data Source={_dbPath}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Timestamp, Entries, TotalSum FROM History ORDER BY Timestamp DESC";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var record = new HistoryRecord
                {
                    Id = reader.GetInt32(0),
                    Timestamp = DateTime.Parse(reader.GetString(1)),
                    TotalSum = reader.GetDouble(3)
                };

                var entriesJson = reader.GetString(2);
                try {
                    var entries = JsonSerializer.Deserialize<List<double>>(entriesJson);
                    if (entries != null) record.Entries = entries;
                } catch { }

                results.Add(record);
            }

            return results;
        }
    }
}

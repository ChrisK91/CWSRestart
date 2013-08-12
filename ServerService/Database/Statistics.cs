using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerService.Database
{
    public sealed class Statistics : Database
    {
        public Statistics(string Filename)
            : base(Filename)
        { }

        protected override void setUpDatabase()
        {
            SQLiteCommand command = new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS statistics (
    ID integer not null primary key autoincrement,
    Timestamp DATETIME DEFAULT CURRENT_TIMESTAMP,
    Runtime TIME,
    CurrentPlayers INTEGER,
    TotalPlayers INTEGER,
    CurrentMemory LONG,
    MaximumMemory LONG,
    Restarts INTEGER
)");
            executeCommand(command);
        }

        /// <summary>
        /// Inserts the given data into the statistics table
        /// </summary>
        /// <param name="Time"></param>
        /// <param name="CurrentPlayers"></param>
        /// <param name="TotalPlayers"></param>
        /// <param name="CurrentMemory"></param>
        /// <param name="MaxMemory"></param>
        /// <param name="Restarts"></param>
        public void InsertStatisticsEntry(TimeSpan Time, int CurrentPlayers, int TotalPlayers, long CurrentMemory, long MaxMemory, int Restarts)
        {
            SQLiteCommand command = new SQLiteCommand(@"INSERT INTO statistics (Runtime, CurrentPlayers, TotalPlayers, CurrentMemory, MaximumMemory, Restarts) VALUES ($runtime, $current, $total, $currentmem, $maxmem, $restart)");

            command.Parameters.AddWithValue("$runtime", Time);
            command.Parameters.AddWithValue("$current", CurrentPlayers);
            command.Parameters.AddWithValue("$total", TotalPlayers);
            command.Parameters.AddWithValue("$currentmem", CurrentMemory);
            command.Parameters.AddWithValue("$maxmem", MaxMemory);
            command.Parameters.AddWithValue("$restart", Restarts);

            executeCommand(command);
        }

        public async Task<List<StatisticsEntry>> GetStatisticEntriesAsync(int limit)
        {
            connection.Open();

            List<StatisticsEntry> ret = new List<StatisticsEntry>();
            SQLiteCommand command = new SQLiteCommand("SELECT ID, Timestamp, Runtime, CurrentPlayers, TotalPlayers, CurrentMemory, MaximumMemory, Restarts FROM statistics ORDER BY Timestamp DESC Limit $limit", connection);
            command.Parameters.AddWithValue("$limit", limit);
            DbDataReader reader = await command.ExecuteReaderAsync();

            while (reader.Read())
            {
                int ID = Convert.ToInt32(reader["ID"]);
                DateTime Timestamp = (DateTime)reader["Timestamp"];
                TimeSpan Runtime = TimeSpan.Parse(reader.GetString(2));
                int CurrentPlayers = Convert.ToInt32(reader["CurrentPlayers"]);
                int TotalPlayers = Convert.ToInt32(reader["TotalPlayers"]);
                long CurrentMemory = Convert.ToInt64(reader["CurrentMemory"]);
                long MaxMemory = Convert.ToInt64(reader["MaximumMemory"]);
                int Restarts = Convert.ToInt32(reader["Restarts"]);

                StatisticsEntry tmp = new StatisticsEntry(ID, Timestamp, Runtime, CurrentPlayers, TotalPlayers, CurrentMemory, MaxMemory, Restarts);
                ret.Add(tmp);
            }

            reader.Close();

            connection.Close();
            return ret;
        }

        public sealed class StatisticsEntry
        {
            public int ID { get; private set; }
            public DateTime TimeStamp { get; private set; }
            public TimeSpan Runtime { get; private set; }
            public int CurrentPlayers { get; private set; }
            public int TotalPlayers { get; private set; }
            public long CurrentMemory { get; private set; }
            public long PeakMemory { get; private set; }
            public int Restarts { get; private set; }

            internal StatisticsEntry(int id, DateTime timestamp, TimeSpan runtime, int currentplayers, int maxplayers, long currentmem, long maxmem, int restarts)
            {
                this.ID = id;
                this.TimeStamp = timestamp;
                this.Runtime = runtime;
                this.CurrentPlayers = currentplayers;
                this.TotalPlayers = maxplayers;
                this.CurrentMemory = currentmem;
                this.PeakMemory = maxmem;
                this.Restarts = restarts;
            }
        }
    }
}

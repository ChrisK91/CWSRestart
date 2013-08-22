using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerService.Database
{
    public sealed class PremiumPlayers : DatabaseBase
    {
        public PremiumPlayers(string filename) : base(filename)
        {
            CheckAndCreateDatabaseFile(filename);
        }

        protected override void SetupDatabase()
        {
            List<SQLiteCommand> l = new List<SQLiteCommand>();

            ///GUID Format: Guid.NewGuid().ToString("N") 

            l.Add(new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS premium (
ID integer not null primary key autoincrement,
IP varchar(39),
GUID varchar(32),
Timestamp DATETIME DEFAULT CURRENT_TIMESTAMP
)"));

            l.Add(new SQLiteCommand(@"CREATE UNIQUE INDEX GUID_Index on premium(GUID)"));

            ExecuteCommand(l);
        }

        public async void AddPremiumPlayerAsync(string ip, Guid guid)
        {
            await Connection.OpenAsync();

            // Check if the player already has an entry
            SQLiteCommand command = new SQLiteCommand("SELECT COUNT(*) FROM premium WHERE GUID = $guid");
            command.Parameters.AddWithValue("$guid", guid.ToString("N"));

            long count = (long)await ExecuteScalarAsync(command);

            if (count == 1)
            {
                command = new SQLiteCommand("UPDATE premium SET ip = $ip, timestamp = datetime('now') WHERE GUID = $guid");
                command.Parameters.AddWithValue("$guid", guid.ToString("N"));
                command.Parameters.AddWithValue("$ip", ip);
                command.Connection = Connection;
                await command.ExecuteNonQueryAsync();
            }
            else
            {
                command = new SQLiteCommand("INSERT INTO premium(IP, GUID) VALUES ($ip, $guid)");
                command.Parameters.AddWithValue("$guid", guid.ToString("N"));
                command.Parameters.AddWithValue("$ip", ip);
                command.Connection = Connection;
                await command.ExecuteNonQueryAsync();
            }

            Connection.Close();
        }

        public async Task<bool> CheckPremiumPlayerAsync(string ip)
        {
            SQLiteCommand command = new SQLiteCommand("SELECT COUNT(*) FROM premium WHERE IP = $ip");
            command.Parameters.AddWithValue("$ip", ip);

            long count = (long)await ExecuteScalarAsync(command);

            return count > 0;
        }

        public void ClearPremiumPlayers()
        {
            SQLiteCommand command = new SQLiteCommand("DELETE FROM premium");
            ExecuteCommand(command);
        }
    }
}

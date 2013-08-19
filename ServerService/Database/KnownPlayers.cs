using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServerService.Database
{
    /// <summary>
    /// A Database containing the names of players
    /// </summary>
    public sealed class KnownPlayers : DatabaseBase
    {
        public KnownPlayers(string filename)
            : base(filename)
        {
            CheckAndCreateDatabaseFile(filename);
        }

        protected override void SetupDatabase()
        {
            List<SQLiteCommand> l = new List<SQLiteCommand>();

            l.Add(new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS names (
ID integer not null primary key autoincrement,
Name varchar(255)
)"));
            l.Add(new SQLiteCommand(@"CREATE UNIQUE INDEX Name_Index on names(Name)"));

            l.Add(new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS ips (
ID integer not null primary key autoincrement,
IP varchar(39)
)"));
            l.Add(new SQLiteCommand(@"CREATE UNIQUE INDEX IP_Index on ips(IP)"));

            l.Add(new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS connectedPlayers (
ID integer not null primary key autoincrement,
IP long not null,
Name long not null,
FOREIGN KEY (Name) REFERENCES names(ID),
FOREIGN KEY (IP) REFERENCES ips(ID)
)"));
            l.Add(new SQLiteCommand(@"CREATE UNIQUE INDEX connectedIP_Index on connectedPlayers(IP)"));
            l.Add(new SQLiteCommand(@"CREATE INDEX connectedName_Index on connectedPlayers(Name)"));

            l.Add(new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS knownPlayers (
ID integer not null primary key autoincrement,
IP long not null,
Name long not null,
FOREIGN KEY (Name) REFERENCES names(ID),
FOREIGN KEY (IP) REFERENCES ips(ID)
)"));

            l.Add(new SQLiteCommand(@"CREATE INDEX knownIP_Index on knownPlayers(IP)"));
            l.Add(new SQLiteCommand(@"CREATE INDEX knownName_Index on knownPlayers(Name)"));

            ExecuteCommand(l);
        }

        /// <summary>
        /// Clears the currently connected players
        /// </summary>
        public void ClearConnectedPlayers()
        {
            SQLiteCommand command = new SQLiteCommand("DELETE FROM connectedPlayers");
            ExecuteCommand(command);
        }

        /// <summary>
        /// Removes the specified player from the connected players
        /// </summary>
        /// <param name="ip">The ip of the player to remove</param>
        public void RemoveConnectedPlayer(string ip)
        {
            long ipId = getIPID(ip);
            SQLiteCommand command = new SQLiteCommand("DELETE FROM connectedPlayers WHERE IP = $ip");
            command.Parameters.AddWithValue("$ip", ipId);
            ExecuteCommand(command);
        }

        /// <summary>
        /// Adds a known name for an ip
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="name"></param>
        public void AddKnownPlayer(string ip, string name)
        {
            long ipId = getIPID(ip);
            long nameId = getNameID(name);

            SQLiteCommand command = new SQLiteCommand("SELECT COUNT(*) FROM knownPlayers WHERE IP = $ipId AND Name = $nameId");
            command.Parameters.AddWithValue("$ipId", ipId);
            command.Parameters.AddWithValue("$nameId", nameId);

            long count = (long)ExecuteScalar(command);

            if (count == 0)
            {
                command = new SQLiteCommand("INSERT INTO knownPlayers(IP, Name) VALUES($ipId, $nameId)");
                command.Parameters.AddWithValue("$ipId", ipId);
                command.Parameters.AddWithValue("$nameId", nameId);

                ExecuteCommand(command);
            }
        }

        /// <summary>
        /// Adds a known name for an ip (async)
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="name"></param>
        public async void AddKnownPlayerAsync(string ip, string name)
        {
            await Connection.OpenAsync();

            long ipId = await getIPIDAsync(ip);
            long nameId = await getNameIDAsync(name);

            SQLiteCommand command = new SQLiteCommand("SELECT COUNT(*) FROM knownPlayers WHERE IP = $ipId AND Name = $nameId");
            command.Parameters.AddWithValue("$ipId", ipId);
            command.Parameters.AddWithValue("$nameId", nameId);

            long count = (long)await ExecuteScalarAsync(command);

            if (count == 0)
            {
                command = new SQLiteCommand("INSERT INTO knownPlayers(IP, Name) VALUES($ipId, $nameId)");
                command.Parameters.AddWithValue("$ipId", ipId);
                command.Parameters.AddWithValue("$nameId", nameId);

                ExecuteCommandAsync(command);
            }

            Connection.Close();
        }

        /// <summary>
        /// Adds a currently connected player
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="name"></param>
        public void AddConnectedPlayer(string ip, string name)
        {
            long ipId = getIPID(ip);
            long nameId = getNameID(name);

            SQLiteCommand command = new SQLiteCommand("INSERT INTO connectedPlayers(IP,Name) VALUES($ipId, $nameId)");

            command.Parameters.AddWithValue("$ipId", ipId);
            command.Parameters.AddWithValue("$nameId", nameId);

            ExecuteCommand(command);
        }

        /// <summary>
        /// Adds a currently connected player (async)
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="name"></param>
        public async void AddConnectedPlayerAsync(string ip, string name)
        {
            await Connection.OpenAsync();
            long ipId = await getIPIDAsync(ip);
            long nameId = await getNameIDAsync(name);

            SQLiteCommand command = new SQLiteCommand("INSERT INTO connectedPlayers(IP,Name) VALUES($ipId, $nameId)");

            command.Parameters.AddWithValue("$ipId", ipId);
            command.Parameters.AddWithValue("$nameId", nameId);

            ExecuteCommandAsync(command);
            Connection.Close();
        }

        private long getIPID(string ip, bool dontInsert = false)
        {
            SQLiteCommand command = new SQLiteCommand("SELECT COUNT(*) FROM ips WHERE IP = $ip");
            command.Parameters.AddWithValue("$ip", ip);

            long count = (long)ExecuteScalar(command);

            if (count == 1)
            {
                command = new SQLiteCommand("SELECT ID FROM ips WHERE IP = $ip");
                command.Parameters.AddWithValue("$ip", ip);

                return (long)ExecuteScalar(command);
            }
            else if (dontInsert)
            {
                return -1;
            }

            command = new SQLiteCommand("INSERT INTO ips(IP) VALUES($ip)");
            command.Parameters.AddWithValue("$ip", ip);

            ExecuteCommand(command);

            command = new SQLiteCommand("SELECT ID FROM ips WHERE IP = $ip");
            command.Parameters.AddWithValue("$ip", ip);

            return (long)ExecuteScalar(command);
        }

        private async Task<long> getIPIDAsync(string ip, bool dontInsert = false)
        {
            SQLiteCommand command = new SQLiteCommand("SELECT COUNT(*) FROM ips WHERE IP = $ip");
            command.Parameters.AddWithValue("$ip", ip);

            long count = (long)await ExecuteScalarAsync(command);

            if (count == 1)
            {
                command = new SQLiteCommand("SELECT ID FROM ips WHERE IP = $ip");
                command.Parameters.AddWithValue("$ip", ip);

                return (long)await ExecuteScalarAsync(command);
            }
            else if (dontInsert)
            {
                return -1;
            }

            command = new SQLiteCommand("INSERT INTO ips(IP) VALUES($ip)");
            command.Parameters.AddWithValue("$ip", ip);

            ExecuteCommandAsync(command);

            command = new SQLiteCommand("SELECT ID FROM ips WHERE IP = $ip");
            command.Parameters.AddWithValue("$ip", ip);

            return (long)await ExecuteScalarAsync(command);
        }

        private long getNameID(string name, bool dontInsert = false)
        {
            SQLiteCommand command = new SQLiteCommand("SELECT COUNT(*) FROM names WHERE Name = $name");
            command.Parameters.AddWithValue("$name", name);

            long count = (long)ExecuteScalar(command);

            if (count == 1)
            {
                command = new SQLiteCommand("SELECT ID FROM names WHERE Name = $name");
                command.Parameters.AddWithValue("$name", name);

                return (long)ExecuteScalar(command);
            }
            else if (dontInsert)
            {
                return -1;
            }

            command = new SQLiteCommand("INSERT INTO names(Name) VALUES($name)");
            command.Parameters.AddWithValue("$name", name);

            ExecuteCommand(command);

            command = new SQLiteCommand("SELECT ID FROM names WHERE Name = $name");
            command.Parameters.AddWithValue("$name", name);

            return (long)ExecuteScalar(command);
        }

        private async Task<long> getNameIDAsync(string name, bool dontInsert = false)
        {
            SQLiteCommand command = new SQLiteCommand("SELECT COUNT(*) FROM names WHERE Name = $name");
            command.Parameters.AddWithValue("$name", name);

            long count = (long)await ExecuteScalarAsync(command);

            if (count == 1)
            {
                command = new SQLiteCommand("SELECT ID FROM names WHERE Name = $name");
                command.Parameters.AddWithValue("$name", name);

                return (long)await ExecuteScalarAsync(command);
            }
            else if (dontInsert)
            {
                return -1;
            }

            command = new SQLiteCommand("INSERT INTO names(Name) VALUES($name)");
            command.Parameters.AddWithValue("$name", name);

            ExecuteCommandAsync(command);

            command = new SQLiteCommand("SELECT ID FROM names WHERE Name = $name");
            command.Parameters.AddWithValue("$name", name);

            return (long)await ExecuteScalarAsync(command);
        }

        /// <summary>
        /// Retreives the name of a connected player
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public string GetConnectedPlayerName(string ip)
        {
            long ipId = getIPID(ip, true);
            SQLiteCommand command = new SQLiteCommand("SELECT COUNT(*) FROM connectedPlayers WHERE IP = $ip");
            command.Parameters.AddWithValue("$ip", ipId);

            long count = (long)ExecuteScalar(command);

            if (count == 0)
                return null;

            command = new SQLiteCommand("SELECT names.Name FROM connectedPlayers LEFT JOIN names ON names.ID = connectedPlayers.Name WHERE IP = $ip");
            command.Parameters.AddWithValue("$ip", ipId);

            return ExecuteScalar(command).ToString();
        }

        /// <summary>
        /// Retreives the name of a connected player (async)
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public async Task<string> GetConnectedPlayerNameAsync(string ip)
        {
            await Connection.OpenAsync();

            long ipId = await getIPIDAsync(ip, true);
            SQLiteCommand command = new SQLiteCommand("SELECT COUNT(*) FROM connectedPlayers WHERE IP = $ip");
            command.Parameters.AddWithValue("$ip", ipId);

            long count = (long)await ExecuteScalarAsync(command);

            if (count == 0)
            {
                Connection.Close();
                return null;
            }

            command = new SQLiteCommand("SELECT names.Name FROM connectedPlayers LEFT JOIN names ON names.ID = connectedPlayers.Name WHERE IP = $ip");
            command.Parameters.AddWithValue("$ip", ipId);

            Connection.Close();
            return (await ExecuteScalarAsync(command)).ToString();
        }

        /// <summary>
        /// Retreives a list of known names for an IP
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public IList<string> GetKnownNames(string ip)
        {
            List<string> ret = new List<string>();
            long ipId = getIPID(ip, true);

            SQLiteCommand command = new SQLiteCommand("SELECT COUNT(*) FROM knownPlayers WHERE IP = $ip");
            command.Parameters.AddWithValue("$ip", ipId);

            long count = (long)ExecuteScalar(command);

            if (count > 0)
            {
                Connection.Open();

                command = new SQLiteCommand("SELECT names.Name as Name FROM knownPlayers LEFT JOIN names ON names.ID = knownPlayers.Name WHERE IP = $ip");
                command.Parameters.AddWithValue("$ip", ipId);
                command.Connection = Connection;

                DbDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string name = (string)reader["Name"];
                    ret.Add(name);
                }

                reader.Close();

                Connection.Close();

                ret.Sort();
            }

            return ret;
        }


        /// <summary>
        /// Retreives a list of known names for an IP (async)
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public async Task<List<string>> GetKnownNamesAsync(string IP)
        {
            await Connection.OpenAsync();

            List<string> ret = new List<string>();
            long ipId = await getIPIDAsync(IP, true);

            SQLiteCommand command = new SQLiteCommand("SELECT COUNT(*) FROM knownPlayers WHERE IP = $ip");
            command.Parameters.AddWithValue("$ip", ipId);

            long count = (long)await ExecuteScalarAsync(command);

            if (count > 0)
            {
                //await connection.OpenAsync();

                command = new SQLiteCommand("SELECT names.Name as Name FROM knownPlayers LEFT JOIN names ON names.ID = knownPlayers.Name WHERE IP = $ip");
                command.Parameters.AddWithValue("$ip", ipId);
                command.Connection = Connection;

                DbDataReader reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    string name = (string)reader["Name"];
                    ret.Add(name);
                }

                reader.Close();

                //connection.Close();

                ret.Sort();
            }

            Connection.Close();
            return ret;
        }
    }
}

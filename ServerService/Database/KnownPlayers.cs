using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServerService.Database
{
    public class KnownPlayers : Database
    {
        public KnownPlayers(string filename)
            : base(filename)
        { }

        protected override void setUpDatabase()
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

            executeCommand(l);
        }

        public void ClearConnectedPlayers()
        {
            SQLiteCommand command = new SQLiteCommand("DELETE FROM connectedPlayers");
            executeCommand(command);
        }

        public void RemoveConnectedPlayer(string ip)
        {
            long ipId = getIPID(ip);
            SQLiteCommand command = new SQLiteCommand("DELETE FROM connectedPlayers WHERE IP = $ip");
            command.Parameters.AddWithValue("$ip", ipId);
            executeCommand(command);
        }

        public void AddKnownPlayer(string ip, string name)
        {
            long ipId = getIPID(ip);
            long nameId = getNameID(name);

            SQLiteCommand command = new SQLiteCommand("SELECT COUNT(*) FROM knownPlayers WHERE IP = $ipId AND Name = $nameId");
            command.Parameters.AddWithValue("$ipId", ipId);
            command.Parameters.AddWithValue("$nameId", nameId);

            long count = (long)executeScalar(command);

            if (count == 0)
            {
                command = new SQLiteCommand("INSERT INTO knownPlayers(IP, Name) VALUES($ipId, $nameId)");
                command.Parameters.AddWithValue("$ipId", ipId);
                command.Parameters.AddWithValue("$nameId", nameId);

                executeCommand(command);
            }
        }

        public void AddConnectedPlayer(string ip, string name)
        {
            long ipId = getIPID(ip);
            long nameId = getNameID(name);

            SQLiteCommand command = new SQLiteCommand("INSERT INTO connectedPlayers(IP,Name) VALUES($ipId, $nameId)");

            command.Parameters.AddWithValue("$ipId", ipId);
            command.Parameters.AddWithValue("$nameId", nameId);

            executeCommand(command);
        }

        private long getIPID(string ip)
        {
            SQLiteCommand command = new SQLiteCommand("SELECT COUNT(*) FROM ips WHERE IP = $ip");
            command.Parameters.AddWithValue("$ip", ip);

            long count = (long)executeScalar(command);

            if (count == 1)
            {
                command = new SQLiteCommand("SELECT ID FROM ips WHERE IP = $ip");
                command.Parameters.AddWithValue("$ip", ip);

                return (long)executeScalar(command);
            }

            command = new SQLiteCommand("INSERT INTO ips(IP) VALUES($ip)");
            command.Parameters.AddWithValue("$ip", ip);

            executeCommand(command);

            command = new SQLiteCommand("SELECT ID FROM ips WHERE IP = $ip");
            command.Parameters.AddWithValue("$ip", ip);

            return (long)executeScalar(command);
        }

        private long getNameID(string name)
        {
            SQLiteCommand command = new SQLiteCommand("SELECT COUNT(*) FROM names WHERE Name = $name");
            command.Parameters.AddWithValue("$name", name);

            long count = (long)executeScalar(command);

            if (count == 1)
            {
                command = new SQLiteCommand("SELECT ID FROM names WHERE Name = $name");
                command.Parameters.AddWithValue("$name", name);

                return (long)executeScalar(command);
            }

            command = new SQLiteCommand("INSERT INTO names(Name) VALUES($name)");
            command.Parameters.AddWithValue("$name", name);

            executeCommand(command);

            command = new SQLiteCommand("SELECT ID FROM names WHERE Name = $name");
            command.Parameters.AddWithValue("$name", name);

            return (long)executeScalar(command);
        }

        public string GetConnectedPlayerName(string ip)
        {
            long ipId = getIPID(ip);
            SQLiteCommand command = new SQLiteCommand("SELECT COUNT(*) FROM connectedPlayers WHERE IP = $ip");
            command.Parameters.AddWithValue("$ip", ipId);

            long count = (long)executeScalar(command);

            if (count == 0)
                return null;

            command = new SQLiteCommand("SELECT names.Name FROM connectedPlayers LEFT JOIN names ON names.ID = connectedPlayers.Name WHERE IP = $ip");
            command.Parameters.AddWithValue("$ip", ipId);

            return executeScalar(command).ToString();
        }

        public List<string> GetKnownNames(string ip)
        {
            List<string> ret = new List<string>();
            long ipId = getIPID(ip);

            SQLiteCommand command = new SQLiteCommand("SELECT COUNT(*) FROM knownPlayers WHERE IP = $ip");
            command.Parameters.AddWithValue("$ip", ipId);

            long count = (long)executeScalar(command);

            if (count > 0)
            {
                connection.Open();

                command = new SQLiteCommand("SELECT names.Name as Name FROM knownPlayers LEFT JOIN names ON names.ID = knownPlayers.Name WHERE IP = $ip");
                command.Parameters.AddWithValue("$ip", ipId);
                command.Connection = connection;

                DbDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string name = (string)reader["Name"];
                    ret.Add(name);
                }

                reader.Close();

                connection.Close();

                ret.Sort();
            }

            return ret;
        }
    }
}

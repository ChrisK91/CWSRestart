using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerService.Database
{
    public abstract class Database
    {
        public string Filename { get; private set; }
        protected SQLiteConnection connection;

        /// <summary>
        /// Initializes the Database. Calls setUpDatabase()
        /// </summary>
        /// <param name="Filename">The database file. Will be created if it doesnt exist</param>
        public Database(string Filename)
        {
            this.Filename = Filename;

            if (!File.Exists(Filename))
                SQLiteConnection.CreateFile(Filename);

            connection = new SQLiteConnection(String.Format("Data Source=\"{0}\"", Filename));

            setUpDatabase();
        }

        protected abstract void setUpDatabase();

        /// <summary>
        /// Will execute the given command on the database
        /// </summary>
        /// <param name="command">The command to execute</param>
        protected void executeCommand(SQLiteCommand command)
        {
            connection.Open();

            command.Connection = connection;
            command.ExecuteNonQuery();

            connection.Close();
        }

        /// <summary>
        /// Will execute the given command on the database asynchronously
        /// </summary>
        /// <param name="command">The command to execute</param>
        protected async void executeCommandAsync(SQLiteCommand command)
        {
            connection.Open();

            command.Connection = connection;
            await command.ExecuteNonQueryAsync();

            connection.Close();
        }

        /// <summary>
        /// Will execute all given commands in a single transaction
        /// </summary>
        /// <param name="commands">The commands to execute</param>
        protected void executeCommand(IEnumerable<SQLiteCommand> commands)
        {
            connection.Open();

            SQLiteCommand begin = new SQLiteCommand("BEGIN TRANSACTION", connection);
            begin.ExecuteNonQuery();

            foreach (SQLiteCommand c in commands)
            {
                c.ExecuteNonQuery();
            }

            SQLiteCommand end = new SQLiteCommand("END TRANSACTION", connection);
            end.ExecuteNonQuery();

            connection.Close();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerService.Database
{
    public abstract class DatabaseBase : IDisposable
    {
        public string DatabaseFile { get; private set; }
        protected SQLiteConnection Connection { get; private set; }

        /// <summary>
        /// Initializes the Database.
        /// </summary>
        /// <param name="filename">The database file. Will be created if it doesnt exist</param>
        protected DatabaseBase(string file)
        {
            this.DatabaseFile = file;

            Connection = new SQLiteConnection(String.Format("Data Source=\"{0}\"", file));
        }

        protected abstract void SetupDatabase();

        protected virtual void CheckAndCreateDatabaseFile(string file)
        {
            if (!File.Exists(file))
            {
                SQLiteConnection.CreateFile(file);
                SetupDatabase();
            }
        }

        /// <summary>
        /// Will execute the given command on the database
        /// </summary>
        /// <param name="command">The command to execute</param>
        protected void ExecuteCommand(SQLiteCommand command)
        {
            bool doClose = InitializeConnection(ref command);

            command.ExecuteNonQuery();

            if (doClose)
                Connection.Close();
        }

        private bool InitializeConnection(ref SQLiteCommand command)
        {
            bool doClose = false;

            if (Connection.State != System.Data.ConnectionState.Open)
            {
                Connection.Open();
                doClose = true;
            }

            initializeCommand(ref command);

            return doClose;
        }

        private async Task<bool> InitializeConnectionAsync()
        {
            bool doClose = false;

            if (Connection.State != System.Data.ConnectionState.Open)
            {
                await Connection.OpenAsync();
                doClose = true;
            }

            return doClose;
        }

        private void initializeCommand(ref SQLiteCommand command)
        {
            if (command.Connection == null)
                command.Connection = Connection;
        }

        /// <summary>
        /// Executes the query, and returns the first column of the first row in the result set returned by the query. Additional columns or rows are ignored. 
        /// </summary>
        /// <param name="command">The command to execute</param>
        /// <returns>The first column of the first row returned by the query</returns>
        protected object ExecuteScalar(SQLiteCommand command)
        {
            bool doClose = InitializeConnection(ref command);

            object ret = command.ExecuteScalar();

            if (doClose)
                Connection.Close();

            return ret;
        }

        protected async Task<object> ExecuteScalarAsync(SQLiteCommand command)
        {
            bool doClose = await InitializeConnectionAsync();
            initializeCommand(ref command);

            object ret = await command.ExecuteScalarAsync();

            if (doClose)
                Connection.Close();

            return ret;
        }

        /// <summary>
        /// Will execute the given command on the database asynchronously
        /// </summary>
        /// <param name="command">The command to execute</param>
        protected async void ExecuteCommandAsync(SQLiteCommand command)
        {
            bool doClose = await InitializeConnectionAsync();
            initializeCommand(ref command);

            await command.ExecuteNonQueryAsync();

            if (doClose)
                Connection.Close();
        }

        /// <summary>
        /// Will execute all given commands in a single transaction
        /// </summary>
        /// <param name="commands">The commands to execute</param>
        protected void ExecuteCommand(IEnumerable<SQLiteCommand> commands)
        {
            if (commands == null)
                throw new ArgumentNullException("commands");

            Connection.Open();

            SQLiteCommand begin = new SQLiteCommand("BEGIN TRANSACTION", Connection);
            begin.ExecuteNonQuery();

            foreach (SQLiteCommand c in commands)
            {
                c.Connection = Connection;
                c.ExecuteNonQuery();
            }

            SQLiteCommand end = new SQLiteCommand("END TRANSACTION", Connection);
            end.ExecuteNonQuery();

            Connection.Close();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (Connection != null)
                Connection.Dispose();
        }
    }
}

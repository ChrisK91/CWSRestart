using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Timers;

namespace ServerService
{
    public sealed class Statistics : INotifyPropertyChanged, IDisposable
    {
        private Timer refresh;
        private DateTime start;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Used to dispose the statistics class
        /// </summary>
        public void Dispose()
        {
            refresh.Stop();
            refresh.Dispose();
            GC.SuppressFinalize(this);
        }


        private ObservableCollection<IPAddress> players = new ObservableCollection<IPAddress>();
        /// <summary>
        /// A list that contains all players, that have visited the server
        /// </summary>
        public ObservableCollection<IPAddress> Players
        {
            get
            {
                return players;
            }
            set
            {
                if (players != value)
                {
                    players = value;
                    notifyPropertyChanged();
                }
            }
        }

        private int loggingIndicator;

        private int restartCount = 0;
        /// <summary>
        /// Indicates how many times the server has been restarted
        /// </summary>
        public int RestartCount
        {
            get
            {
                return restartCount;
            }
            private set
            {
                if (restartCount != value)
                {
                    restartCount = value;
                    notifyPropertyChanged();
                }
            }
        }

        private string logFolder = "";
        /// <summary>
        /// Indicates, where the LogFile should be stored
        /// </summary>
        public string LogFolder
        {
            get
            {
                return logFolder;
            }
            set
            {
                if (logFolder != value)
                {
                    logFolder = value;
                    notifyPropertyChanged();
                }
            }
        }


        private bool enabled = false;
        /// <summary>
        /// Indicates if the Statistics are updated
        /// </summary>
        public bool Enabled
        {
            get
            {
                return enabled;
            }
            private set
            {
                if (enabled != value)
                {
                    enabled = value;
                    notifyPropertyChanged();
                    notifyPropertyChanged("ButtonText");
                }
            }
        }

        public string ButtonText
        {
            get
            {
                if (enabled)
                    return "Disable statistics";
                else
                    return "Enable statistics";
            }
        }

        private ObservableCollection<IPAddress> connectedplayers = new ObservableCollection<IPAddress>();
        /// <summary>
        /// A list that contains all players, that have visited the server
        /// </summary>
        public ObservableCollection<IPAddress> ConnectedPlayers
        {
            get
            {
                return connectedplayers;
            }
            set
            {
                if (connectedplayers != value)
                {
                    connectedplayers = value;
                    notifyPropertyChanged();
                }
            }
        }

        private TimeSpan runtime = new TimeSpan(0);
        /// <summary>
        /// The duration of the current sessions
        /// </summary>
        public TimeSpan Runtime
        {
            get
            {
                return runtime;
            }
            set
            {
                if (runtime != value)
                {
                    runtime = value;
                    notifyPropertyChanged();
                }
            }
        }

        public void UpdateRuntime()
        {
            Runtime = DateTime.Now.Subtract(start);
        }

        private long peakMemoryUsage = 0;
        /// <summary>
        /// Contains the maximum amount of memory used by the server
        /// </summary>
        public long PeakMemoryUsage
        {
            get
            {
                return peakMemoryUsage;
            }
            private set
            {
                if (peakMemoryUsage != (value / 1024 / 1024))
                {
                    peakMemoryUsage = value / 1024 / 1024;
                    notifyPropertyChanged();
                }
            }
        }

        private long currentMemoryUsage = 0;
        /// <summary>
        /// Contains the current amount of memory used by the server
        /// </summary>
        public long CurrentMemoryUsage
        {
            get
            {
                return currentMemoryUsage;
            }
            private set
            {
                if (currentMemoryUsage != value / 1024 / 1024)
                {
                    currentMemoryUsage = value / 1024 / 1024;
                    notifyPropertyChanged();
                }
            }
        }

        public void UpdateCurrentMemoryUsage()
        {
            CurrentMemoryUsage = (Helper.General.Server != null) ? Helper.General.Server.PrivateMemorySize64 : 0;
        }

        /// <summary>
        /// Initializes the statistics and starts the autorefresh
        /// </summary>
        /// <param name="timeout">Update interval of the statistics (in ms)</param>
        public Statistics(int timeout)
        {
            initialize(timeout, true);
        }

        /// <summary>
        /// Initializes the statistics
        /// </summary>
        /// <param name="timeout">Update interval of the statistics (in ms)</param>
        /// <param name="autostart">If the timer should be started</param>
        public Statistics(int timeout, bool autostart)
        {
            initialize(timeout, autostart);
        }

        private void initialize(int timeout, bool autostart)
        {
            start = DateTime.Now;

            Helper.General.ServerRestarted += Helper_ServerRestarted;

#if DEBUG

            start = start.Subtract(new TimeSpan(5, 0, 0, 0));

            Players.Add(IPAddress.Parse("192.168.178.2"));
            Players.Add(IPAddress.Parse("192.168.178.3"));
            Players.Add(IPAddress.Parse("192.168.178.4"));
            Players.Add(IPAddress.Parse("192.168.178.5"));
            Players.Add(IPAddress.Parse("192.168.178.6"));

#endif

            refresh = new Timer(Settings.Instance.StatisticsInterval);
            refresh.Elapsed += refresh_Elapsed;

            if (autostart)
            {
                refresh.Start();
                Enabled = true;
            }
        }

        void Helper_ServerRestarted(object sender, EventArgs e)
        {
            increaseRestartCount();
        }

        /// <summary>
        /// Starts the statistics timer
        /// </summary>
        public void Start()
        {
            refresh.Start();

            if (LogFolder == "")
                Logging.OnLogMessage("To save your statistics, use the button on the left", Logging.MessageType.Info);

            Enabled = true;
        }

        /// <summary>
        /// Stops the statistics timer
        /// </summary>
        public void Stop()
        {
            refresh.Stop();
            Enabled = false;
        }

        /// <summary>
        /// Updates the statistics
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void refresh_Elapsed(object sender, ElapsedEventArgs e)
        {
            refresh.Stop();

            UpdateRuntime();

            if (Validator.Instance.IsRunning())
            {
                UpdatePlayers();

                if (Helper.General.Server != null)
                {
                    Helper.General.Server.Refresh();

                    if (PeakMemoryUsage <= (Helper.General.Server.PrivateMemorySize64 / 1024 / 1024))
                        PeakMemoryUsage = Helper.General.Server.PrivateMemorySize64;

                    UpdateCurrentMemoryUsage();
                }

                if ((loggingIndicator % Settings.Instance.SaveStatisticsEvery) == 0)
                {
                    AccessControl.Instance.Enforce();

                    if (LogFolder != "")
                    {
                        try
                        {
                            string targetFile = Path.Combine(LogFolder, String.Format("{0}.{1}", start.ToString("yyyy-MM-dd_HH-mm-ss"), "csv"));

                            StreamWriter sw = null;

                            if (!File.Exists(targetFile))
                            {
                                File.Create(targetFile).Close();
                                sw = File.AppendText(targetFile);
                                sw.WriteLine("Timestamp;Runtime;Current players;Total players;Current memory;Max memory;Number of restarts");
                            }

                            if (sw == null)
                                sw = File.AppendText(targetFile);

                            sw.WriteLine("{0};{1};{2};{3};{4};{5};{6}",
                                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                String.Format("{0}:{1:00}:{2:00}", Math.Floor(Runtime.TotalHours), Runtime.Minutes, Runtime.Seconds),
                                ConnectedPlayers.Count,
                                Players.Count,
                                CurrentMemoryUsage,
                                PeakMemoryUsage,
                                RestartCount);

                            sw.Close();
                            loggingIndicator = 1;
                        }
                        catch (Exception ex)
                        {
                            Logging.OnLogMessage(String.Format("Could not write to file: {0}", ex.Message), Logging.MessageType.Error);
                            Logging.OnLogMessage("The log is now incomplete.", Logging.MessageType.Warning);
                        }
                    }
                }
                else
                {
                    loggingIndicator++;
                }
            }

            refresh.Start();
        }

        /// <summary>
        /// Updates the current players
        /// </summary>
        /// <returns></returns>
        public void UpdatePlayers()
        {
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] connectionInformation = ipGlobalProperties.GetActiveTcpConnections();

            IEnumerator enumerator = connectionInformation.GetEnumerator();

            int count = 0;

            ObservableCollection<IPAddress> current = new ObservableCollection<IPAddress>();
            ObservableCollection<IPAddress> all = new ObservableCollection<IPAddress>(Players);

            while (enumerator.MoveNext())
            {
                TcpConnectionInformation info = (TcpConnectionInformation)enumerator.Current;

                if (info.LocalEndPoint.Port == 12345 && info.State == TcpState.Established)
                {
                    count++;

                    if (!all.Contains(info.RemoteEndPoint.Address))
                        all.Add(info.RemoteEndPoint.Address);

                    current.Add(info.RemoteEndPoint.Address);
                }
            }

#if DEBUG
            //Time for some debug data

            Random rnd = new Random();
            int playersToGenerate = rnd.Next(1, 10);

            for (int i = 0; i < playersToGenerate; i++)
            {
                int a = rnd.Next(0, 255);


                IPAddress tmp;
                string ip = "127.0.0." + a.ToString();

                if (IPAddress.TryParse(ip, out tmp))
                {
                    if (!current.Contains(tmp))
                        current.Add(tmp);

                    if (!all.Contains(tmp))
                        all.Add(tmp);
                }
            }
#endif

            ConnectedPlayers = current;
            Players = all;
        }

        private void increaseRestartCount()
        {
            RestartCount++;
        }

        private void notifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

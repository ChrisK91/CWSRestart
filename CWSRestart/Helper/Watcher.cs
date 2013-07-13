using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace CWSRestart.Helper
{
    public sealed class Watcher : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private static readonly Watcher instance = new Watcher();

        Timer watcher;

        public void Dispose()
        {
            watcher.Stop();
            watcher.Dispose();
            GC.SuppressFinalize(this);
        }


        private Watcher()
        {
            watcher = new Timer(1000);
            watcher.Elapsed += watcher_Elapsed;
        }

        async void watcher_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (ServerService.Helper.Working)
            {
                IsBlocked = true;
            }
            else
            {
                CurrentStep++;
                IsBlocked = false;

                if (CurrentStep > IntervallSeconds)
                {
                    watcher.Stop();
                    IsBlocked = true;

                    Helper.Logging.OnLogMessage("Time to check if the server is still running", ServerService.Logging.MessageType.Info);

                    ServerService.Validator.ServerErrors errors = await ServerService.Validator.Instance.Validates(ServerService.Settings.Instance.IgnoreAccess);

                    if (errors != 0)
                    {
                        Helper.Logging.OnLogMessage("A restart is required", ServerService.Logging.MessageType.Info);

                        if (!errors.HasFlag(ServerService.Validator.ServerErrors.ProcessDead))
                        {
                            ServerService.Helper.RestartServer();
                        }
                        else
                        {
                            ServerService.Helper.StartServer();
                        }
                    }

                    CurrentStep = 0;
                    IsBlocked = false;
                    watcher.Start();
                }
            }
        }

        public static Watcher Instance
        {
            get
            {
                return instance;
            }
        }

        #region intervall
        private UInt32 intervallSeconds = 60;

        public UInt32 IntervallSeconds
        {
            get
            {
                return intervallSeconds;
            }
            set
            {
                if (value == 0)
                    value = 60;

                intervallSeconds = value;
                notifyPropertyChanged();
            }
        }
        #endregion

        #region isRunning
        private bool isRunning = false;

        public bool IsRunning
        {
            get
            {
                return isRunning;
            }
            private set
            {
                isRunning = value;

                if (isRunning)
                    ButtonText = "Stop watcher";
                else
                    ButtonText = "Start watcher";

                notifyPropertyChanged();
            }
        }

        private string buttonText = "Start watcher";

        public string ButtonText
        {
            get
            {
                return buttonText;
            }
            private set
            {
                buttonText = value;
                notifyPropertyChanged();
            }
        }
        #endregion

        #region isBlocked
        private bool isBlocked = false;

        public bool IsBlocked
        {
            get
            {
                return isBlocked;
            }
            private set
            {
                isBlocked = value;
                notifyPropertyChanged();
            }
        }
        #endregion

        #region current step
        private int currentStep = 0;
        public int CurrentStep
        {
            get
            {
                return currentStep;
            }
            private set
            {
                currentStep = value;
                notifyPropertyChanged();
            }
        }
        #endregion

        public void Toggle()
        {
            if (!watcher.Enabled)
            {
                Logging.OnLogMessage("Watcher started", ServerService.Logging.MessageType.Info);
                watcher.Start();
            }
            else
            {
                Logging.OnLogMessage("Watcher stopped", ServerService.Logging.MessageType.Info);
                watcher.Stop();
            }

            IsRunning = !IsRunning;
        }

        private void notifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

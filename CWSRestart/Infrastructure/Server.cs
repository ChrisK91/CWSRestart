using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CWSRestart.Infrastructure
{
    public class Server : INotifyPropertyChanged
    {
        private volatile bool _shouldStop = false;
        public event PropertyChangedEventHandler PropertyChanged;

        public static Server Instance
        {
            get
            {
                return instance;
            }
        }

        private static Server instance = new Server();

        private Server()
        {
        }

        NamedPipeServerStream serverStream;
        volatile EventWaitHandle wait;

        private void doServerWork()
        {
            IsRunning = true;
            serverStream = new NamedPipeServerStream("CWSRestartServer", PipeDirection.InOut, 4, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);

            Helper.Logging.OnLogMessage("Starting CWSRestartServer for process communication", ServerService.Logging.MessageType.Info);

            while (!_shouldStop)
            {
                wait = new EventWaitHandle(false, EventResetMode.ManualReset);

                serverStream.BeginWaitForConnection(ar =>
                    {
                        try
                        {
                            serverStream.EndWaitForConnection(ar);
                            if (serverStream.IsConnected)
                            {
                                Helper.Logging.OnLogMessage("Module connected", ServerService.Logging.MessageType.Info);
                            }
                            serverStream.Close();
                            wait.WaitOne();
                        }
                        catch (ObjectDisposedException)
                        {
                            Helper.Logging.OnLogMessage("CWSRestartServer has been stopped", ServerService.Logging.MessageType.Info);
                            wait.Set();
                        }
                    }, null);

                wait.WaitOne();
            }

            serverStream.Close();
            IsRunning = false;
        }

        private void clientConnected(IAsyncResult ar)
        {
            using (NamedPipeServerStream serverStream = new NamedPipeServerStream("CWSRestartServer", PipeDirection.In, 4, PipeTransmissionMode.Message, PipeOptions.Asynchronous))
            {
                serverStream.EndWaitForConnection(ar);
            }
        }

        private Thread mainServer;

        private bool isRunning = false;
        public bool IsRunning
        {
            get
            {
                return isRunning;
            }
            set
            {
                if (isRunning != value)
                {
                    isRunning = value;
                    notifyPropertyChanged();
                    notifyPropertyChanged("ButtonText");
                }
            }
        }

        public string ButtonText
        {
            get
            {
                return (isRunning) ? "Stop CWSProtocol" : "Start CWSProtocol";
            }
        }

        private void notifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public void ToggleServer()
        {
            if (mainServer == null || !mainServer.IsAlive)
            {
                _shouldStop = false;
                ThreadStart start = new ThreadStart(doServerWork);
                mainServer = new Thread(start);
                mainServer.Start();
            }
            else
            {
                if (wait != null)
                    wait.Set();

                _shouldStop = true;
            }
        }
    }
}

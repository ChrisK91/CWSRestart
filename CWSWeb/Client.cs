using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CWSWeb
{
    class Client
    {
        NamedPipeClientStream client = new NamedPipeClientStream(".", "CWSRestartServer", PipeDirection.InOut, PipeOptions.None, System.Security.Principal.TokenImpersonationLevel.Impersonation);

        private bool _shouldStop = false;
        private EventWaitHandle wait;

        public bool IsRunning { get; private set; }

        public void Connect()
        {
            if (!IsRunning)
            {
                Thread worker = new Thread(() =>
                {
                    try
                    {
                        IsRunning = true;
                        _shouldStop = false;

                        wait = new EventWaitHandle(false, EventResetMode.ManualReset);
                        client.Connect(1000);
                        if (client.IsConnected)
                        {
                            while (!_shouldStop)
                            {
                                byte[] m_buffer = new byte[256];
                                client.BeginRead(m_buffer, 0, 255, ir =>
                                {
                                    try
                                    {
                                        client.EndRead(ir);
                                        wait.Set();
                                    }
                                    catch (Exception ex)
                                    {
                                        if (!(ex is ArgumentException))
                                            Debugger.Break();

                                    }
                                }, null);
                                wait.WaitOne();
                            }
                        }

                        IsRunning = false;
                    }
                    catch (Exception ex)
                    {
                        if (ex is TimeoutException)
                            Console.WriteLine("Unable to connect to CWSRestart. Make sure that the process communication is enabled in CWSRestart");
                        else
                            System.Diagnostics.Debugger.Break();
                        IsRunning = false;
                    }
                });

                worker.Start();
            }
            else
            {
                Console.WriteLine("Communication is already running.");
            }
        }

        public void Stop()
        {
            if (IsRunning)
            {
                _shouldStop = true;
                wait.Set();
                client.Close();
            }
            else
            {
                Console.WriteLine("Communication is not running.");
            }
        }
    }
}

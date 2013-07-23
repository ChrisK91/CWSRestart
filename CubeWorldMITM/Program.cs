using CubeWorldMITM.Networking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CubeWorldMITM
{
    class Program
    {
        private static TcpListener mitm;
        private static volatile bool shouldExit = false;
        private static EventWaitHandle wait;

        static void Main(string[] args)
        {
            mitm = new TcpListener(IPAddress.Any, 12345);

            Thread listenerThread = new Thread(new ThreadStart(ConnectionLoop));
            listenerThread.Start();

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            shouldExit = true;
            listenerThread.Abort();
        }

        private static void ConnectionLoop()
        {
            while (!shouldExit)
            {
                wait = new EventWaitHandle(false, EventResetMode.ManualReset);
                mitm.Start();
                mitm.BeginAcceptTcpClient(OnConnect, null);

                wait.WaitOne();
            }
        }

        private static void OnConnect(IAsyncResult ar)
        {
            TcpClient client = mitm.EndAcceptTcpClient(ar);
            wait.Set();

            TcpClient toServer = new TcpClient(IPAddress.Loopback.ToString(), 12346);

            NetworkStream stream = client.GetStream();
            NetworkStream server = toServer.GetStream();

            bool clientFinished = true;
            bool serverFinished = true;

            while (!shouldExit)
            {
                EventWaitHandle mitmHandle = new EventWaitHandle(false, EventResetMode.ManualReset);

                if (clientFinished)
                {
                    byte[] buffer = new byte[1024];
                    clientFinished = false;
                    stream.BeginRead(buffer, 0, buffer.Length, sAr =>
                    {
                        int bytesRead = stream.EndRead(sAr);
                        server.BeginWrite(buffer, 0, bytesRead, wAr =>
                        {
                            server.EndWrite(wAr);
                        }, null);
                        clientFinished = true;
                        mitmHandle.Set();
                    }, null);
                }

                if (serverFinished)
                {
                    byte[] buffer = new byte[1024];
                    serverFinished = false;
                    server.BeginRead(buffer, 0, buffer.Length, sAr =>
                    {
                        int bytesRead = server.EndRead(sAr);
                        stream.BeginWrite(buffer, 0, bytesRead, wAr =>
                        {
                            stream.EndWrite(wAr);
                        }, null);
                        serverFinished = true;
                        mitmHandle.Set();
                    }, null);
                }

                mitmHandle.WaitOne();
            }

        }
    }
}

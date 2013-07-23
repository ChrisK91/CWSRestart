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

        private static List<MITMMessageHandler> establishedConnections = new List<MITMMessageHandler>();

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

            NetworkStream clientStream = client.GetStream();
            NetworkStream serverStream = toServer.GetStream();

            Console.Write("{0} connected to {1}", client.Client.RemoteEndPoint.ToString(), client.Client.LocalEndPoint.ToString());

            MITMMessageHandler handler = new MITMMessageHandler(clientStream, serverStream);
            establishedConnections.Add(handler);
        }
    }
}

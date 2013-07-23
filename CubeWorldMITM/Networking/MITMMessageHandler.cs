using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CubeWorldMITM.Networking
{
    internal class MITMMessageHandler
    {
        private NetworkStream client;
        private NetworkStream server;

        private const int BUFFERSIZE = 8092;
        private const int IntSize = 4;

        private byte[] clientReceiveBuffer = new byte[BUFFERSIZE];
        private byte[] serverReceiveBuffer = new byte[BUFFERSIZE];

        private byte[] clientSendBuffer = new byte[BUFFERSIZE];
        private byte[] serverSendBuffer = new byte[BUFFERSIZE];

        private byte[] clientPackageBuffer = new byte[IntSize];

        public MITMMessageHandler(NetworkStream Client, NetworkStream Server)
        {
            client = Client;
            server = Server;

            client.BeginRead(clientPackageBuffer, 0, IntSize, new AsyncCallback(ClientReceivedIdCallback), null);
            server.BeginRead(serverReceiveBuffer, 0, BUFFERSIZE, new AsyncCallback(ServerReceivedCallback), null);
        }

        private void ServerReceivedCallback(IAsyncResult ar)
        {
            try
            {
                int numBytes = server.EndRead(ar);

                if (numBytes == 0)
                {
                    throw new Exception("No bytes have been received: disconnect.");
                }
                else
                {
                    Array.Copy(serverReceiveBuffer, 0, clientSendBuffer, 0, numBytes);
                    client.BeginWrite(clientSendBuffer, 0, numBytes, new AsyncCallback(ClientWriteCallback), null);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void ClientReceivedIdCallback(IAsyncResult ar)
        {
            try
            {
                int numBytes = client.EndRead(ar);

                if (numBytes == 0)
                {
                    throw new Exception("No bytes have been received: disconnect.");
                }
                else if (numBytes == 4)
                {
                    Array.Copy(clientPackageBuffer, 0, serverSendBuffer, 0, numBytes);
                    server.BeginWrite(serverSendBuffer, 0, numBytes, new AsyncCallback(ServerWriteCallback), null);

                    byte[] idArray = new byte[4];
                    Array.Copy(clientReceiveBuffer, 0, idArray, 0, 4);

                    int packetId = BitConverter.ToInt32(idArray, 0);

                    if (packetId == 0)
                    {
                        //Entity update
                        byte[] dataLength = new byte[4];
                        Array.Copy(clientReceiveBuffer, 4, dataLength, 0, 4);

                        int lenght = BitConverter.ToInt32(dataLength, 0);
                        if (lenght > 0 && lenght < 1000)
                        {
                            byte[] compressed = new byte[lenght];
                            Array.Copy(clientReceiveBuffer, 8, compressed, 0, lenght);

                            byte[] decompressed = Helper.ZLibHelper.UncompressBytes(compressed);
                        }
                    }
                }
                else
                {
                    throw new Exception("Unexpected package");
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void ServerWriteCallback(IAsyncResult ar)
        {
            server.EndWrite(ar);
            client.BeginRead(clientReceiveBuffer, 0, BUFFERSIZE, new AsyncCallback(ClientReceivedIdCallback), null);
        }

        private void ClientWriteCallback(IAsyncResult ar)
        {
            client.EndWrite(ar);
            server.BeginRead(serverReceiveBuffer, 0, BUFFERSIZE, new AsyncCallback(ServerReceivedCallback), null);
        }
    }
}

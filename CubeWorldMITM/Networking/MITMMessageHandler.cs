using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CubeWorldMITM.Networking
{
    internal class MITMMessageHandler
    {
        private NetworkStream client;
        private NetworkStream server;

        private const int BUFFERSIZE = 8092;
        
        public const int IntSize = sizeof(int);
        public const int UIntSize = sizeof(UInt32);
        public const int Int64Size = sizeof(Int64);
        public const int FloatSize = sizeof(float);
        public const int UInt16Size = sizeof(UInt16);
        public const int UInt32Size = sizeof(UInt32);
        public const int UInt64Size = sizeof(UInt64);

        private int sendDataLength = 0;

        private byte[] clientReceiveBuffer;
        private byte[] serverReceiveBuffer = new byte[BUFFERSIZE];

        private byte[] clientSendBuffer = new byte[BUFFERSIZE];
        private byte[] serverSendBuffer;

        private byte[] packageIntReceiveBuffer = new byte[IntSize];
        private byte[] packageIntSendBuffer = new byte[IntSize];

        public bool ClientIdentified { get; private set; }
        public string Name { get; private set; }
        public string IP { get; private set; }
        public uint Level { get; private set; }

        public bool Connected { get; private set; }

        private int CurrentPackageId = -1;
        private bool dataLengthExpected = false;

        public Action<MITMMessageHandler> OnClientIdentified;
        public Action<MITMMessageHandler> OnClientDisconnected;

        public MITMMessageHandler(NetworkStream Client, NetworkStream Server, string Ip)
        {
            try
            {
                Connected = true;
                client = Client;
                server = Server;
                this.IP = Ip;

                ClientIdentified = false;

                client.BeginRead(packageIntReceiveBuffer, 0, IntSize, new AsyncCallback(ClientIDReceivedCallback), null);
                server.BeginRead(serverReceiveBuffer, 0, BUFFERSIZE, new AsyncCallback(ServerReceivedCallback), null);
            }
            catch (Exception ex)
            {
                Disconnect();
                logError(ex);
            }
        }

        private void ClientIDReceivedCallback(IAsyncResult ar)
        {
            try
            {
                int numBytes = client.EndRead(ar);

                if (numBytes == 0)
                {
                    throw new Exception("No ID received.");
                }
                else
                {
                    Array.Copy(packageIntReceiveBuffer, 0, packageIntSendBuffer, 0, numBytes);

                    int id = BitConverter.ToInt32(packageIntReceiveBuffer, 0);
                    Console.WriteLine("Recevied package {0}", id);

                    //server.BeginWrite(packageIntSendBuffer, 0, numBytes, ServerIDSentCallback, null);

                    dataLengthExpected = false;

                    switch (id)
                    {
                        case 0:
                        case 10:
                            //Variable data length
                            dataLengthExpected = true;
                            break;

                        case 6:
                            dataLengthExpected = false;
                            sendDataLength = 0;

                            sendDataLength += IntSize; //ChunkX
                            sendDataLength += IntSize; //ChunkY
                            sendDataLength += IntSize; //ItemIndex
                            sendDataLength += UIntSize; //Something4
                            sendDataLength += 1; //InteractType
                            sendDataLength += 1; //Something6
                            sendDataLength += UInt16Size; //Something7
                            break;

                        case 17:
                            dataLengthExpected = false;
                            sendDataLength = 0;
                            sendDataLength += IntSize;
                            break;

                        default:
                            Console.WriteLine("Unknown packet {0}", id);
                            break;
                    }

                    CurrentPackageId = id;
                    server.BeginWrite(packageIntSendBuffer, 0, numBytes, ServerIDSentCallback, null);
                }
            }
            catch (Exception ex)
            {
                Disconnect();
                logError(ex);
            }
        }

        private void ServerIDSentCallback(IAsyncResult ar)
        {
            try
            {
                server.EndWrite(ar);

                if (dataLengthExpected)
                    client.BeginRead(packageIntReceiveBuffer, 0, IntSize, ClientLengthReceived, null);
                else
                    ClientBeginListenForData();
            }
            catch (Exception ex)
            {
                Disconnect();
                logError(ex);
            }
        }

        private void ClientLengthReceived(IAsyncResult ar)
        {
            try
            {
                int numBytes = client.EndRead(ar);

                if (numBytes == 0)
                {
                    throw new Exception("No datalength received.");
                }
                else
                {
                    Array.Copy(packageIntReceiveBuffer, 0, packageIntSendBuffer, 0, numBytes);
                    int length = BitConverter.ToInt32(packageIntReceiveBuffer, 0);

                    if (length > 0)
                    {
                        Console.WriteLine("Length: {0}", length);

                        server.BeginWrite(packageIntSendBuffer, 0, numBytes, ServerLengthSentCallback, null);

                        sendDataLength = length;
                    }
                    else
                    {
                        server.BeginWrite(packageIntSendBuffer, 0, numBytes, ServerDataSentCallback, null);
                    }
                }
            }
            catch (Exception ex)
            {
                logError(ex);
            }
        }

        private void ServerLengthSentCallback(IAsyncResult ar)
        {
            try
            {
                server.EndWrite(ar);

                ClientBeginListenForData();
            }
            catch (Exception ex)
            {
                Disconnect();
                logError(ex);
            }
        }

        private void ClientBeginListenForData()
        {
            try
            {
                clientReceiveBuffer = new byte[sendDataLength];
                serverSendBuffer = new byte[sendDataLength];

                client.BeginRead(clientReceiveBuffer, 0, sendDataLength, ClientDataReceivedCallback, null);
            }
            catch (Exception ex)
            {
                Disconnect();
                logError(ex);
            }
        }

        private void ClientDataReceivedCallback(IAsyncResult ar)
        {
            try
            {
                int numBytes = client.EndRead(ar);
                Console.WriteLine("Received {0} bytes", numBytes);

                Array.Copy(clientReceiveBuffer, 0, serverSendBuffer, 0, numBytes);

                #region parse client info
                if (CurrentPackageId == 0)
                {
                    byte[] uncompressedData = Helper.ZLibHelper.UncompressBytes(clientReceiveBuffer);

                    using (BinaryReader reader = new BinaryReader(new MemoryStream(uncompressedData)))
                    {
                        ulong id = reader.ReadUInt64();


                        byte[] LastBitmask = reader.ReadBytes(8);
                        BitArray bitArray = new BitArray(LastBitmask);

                        if (bitArray.Get(0))
                        {
                            reader.ReadInt64();
                            reader.ReadInt64();
                            reader.ReadInt64();
                        }
                        if (bitArray.Get(1))
                        {
                            reader.ReadSingle();
                            reader.ReadSingle();
                            reader.ReadSingle();
                        }
                        if (bitArray.Get(2))
                        {
                            reader.ReadSingle();
                            reader.ReadSingle();
                            reader.ReadSingle();
                        }
                        if (bitArray.Get(3))
                        {
                            reader.ReadSingle();
                            reader.ReadSingle();
                            reader.ReadSingle();
                        }
                        if (bitArray.Get(4))
                        {
                            reader.ReadSingle();
                            reader.ReadSingle();
                            reader.ReadSingle();
                        }
                        if (bitArray.Get(5))
                        {
                            reader.ReadSingle();
                        }
                        if (bitArray.Get(6))
                        {
                            reader.ReadUInt32();
                        }
                        if (bitArray.Get(7))
                        {
                            reader.ReadByte();
                        }
                        if (bitArray.Get(8))
                        {
                            reader.ReadUInt32();
                        }
                        if (bitArray.Get(9))
                        {
                            reader.ReadByte();
                        }
                        if (bitArray.Get(10))
                        {
                            reader.ReadUInt32();
                        }
                        if (bitArray.Get(11))
                        {
                            reader.ReadUInt32();
                        }
                        if (bitArray.Get(12))
                        {
                            reader.ReadUInt32();
                        }
                        if (bitArray.Get(13))
                        {
                            reader.ReadByte();
                            reader.ReadByte();
                            reader.ReadByte();
                            reader.ReadByte();
                            reader.ReadByte();
                            reader.ReadByte(); // skip 1
                            reader.ReadByte();
                            reader.ReadByte();
                            reader.ReadSingle();
                            reader.ReadSingle();
                            reader.ReadSingle();
                            reader.ReadUInt16();
                            reader.ReadUInt16();
                            reader.ReadUInt16();
                            reader.ReadUInt16();
                            reader.ReadUInt16();
                            reader.ReadUInt16();
                            reader.ReadUInt16();
                            reader.ReadUInt16();
                            reader.ReadSingle();
                            reader.ReadSingle();
                            reader.ReadSingle();
                            reader.ReadSingle();
                            reader.ReadSingle();
                            reader.ReadSingle();
                            reader.ReadSingle();
                            reader.ReadSingle();
                            reader.ReadSingle();
                            reader.ReadSingle();
                            reader.ReadSingle();
                            reader.ReadSingle();
                            reader.ReadSingle();
                            reader.ReadSingle();
                            reader.ReadSingle();
                            reader.ReadSingle();
                            reader.ReadSingle();
                            reader.ReadSingle();
                            reader.ReadSingle();
                            reader.ReadSingle();
                            reader.ReadSingle();
                            reader.ReadSingle();
                            reader.ReadSingle();
                            reader.ReadSingle();
                            reader.ReadSingle();
                            reader.ReadSingle();
                            reader.ReadSingle();
                            reader.ReadSingle();
                            reader.ReadSingle();
                            reader.ReadSingle();
                            reader.ReadSingle();
                            reader.ReadSingle();
                            reader.ReadSingle();
                            reader.ReadSingle();
                        }
                        if (bitArray.Get(14))
                        {
                            reader.ReadByte();
                            reader.ReadByte();
                        }
                        if (bitArray.Get(15))
                        {
                            reader.ReadUInt32();
                        }
                        if (bitArray.Get(16))
                        {
                            reader.ReadInt32();
                        }
                        if (bitArray.Get(17))
                        {
                            reader.ReadUInt32();
                        }
                        if (bitArray.Get(18))
                        {
                            reader.ReadUInt32();
                        }
                        if (bitArray.Get(19))
                        {
                            reader.ReadUInt32();
                        }
                        if (bitArray.Get(20))
                        {
                            reader.ReadSingle();
                        }
                        if (bitArray.Get(21))
                        {
                            reader.ReadByte();
                        }
                        if (bitArray.Get(22))
                        {
                            reader.ReadByte();
                        }
                        if (bitArray.Get(23))
                        {
                            reader.ReadSingle();
                        }
                        if (bitArray.Get(24))
                        {
                            reader.ReadUInt32();
                            reader.ReadUInt32();
                            reader.ReadUInt32();
                        }
                        if (bitArray.Get(25))
                        {
                            reader.ReadUInt32();
                            reader.ReadUInt32();
                            reader.ReadUInt32();
                        }
                        if (bitArray.Get(26))
                        {
                            reader.ReadSingle();
                            reader.ReadSingle();
                            reader.ReadSingle();
                        }
                        if (bitArray.Get(27))
                        {
                            reader.ReadSingle();
                        }
                        if (bitArray.Get(28))
                        {
                            reader.ReadSingle();
                        }
                        if (bitArray.Get(29))
                        {
                            reader.ReadSingle();
                        }
                        if (bitArray.Get(30))
                        {
                            reader.ReadSingle();
                            reader.ReadSingle();
                            reader.ReadSingle();
                            reader.ReadSingle();
                            reader.ReadSingle();
                        }
                        if (bitArray.Get(31))
                        {
                            reader.ReadByte();
                        }
                        if (bitArray.Get(32))
                        {
                            reader.ReadByte();
                        }
                        if (bitArray.Get(33))
                        {
                            Level = reader.ReadUInt32();
                            Console.WriteLine("Level {0}", Level);
                        }
                        if (bitArray.Get(34))
                        {
                            reader.ReadUInt32();
                        }
                        if (bitArray.Get(35))
                        {
                            reader.ReadUInt64();
                        }
                        if (bitArray.Get(36))
                        {
                            reader.ReadUInt32();
                            reader.ReadUInt32();
                        }
                        if (bitArray.Get(37))
                        {
                            reader.ReadByte();
                        }
                        if (bitArray.Get(38))
                        {
                            reader.ReadUInt32();
                        }
                        if (bitArray.Get(39))
                        {
                            reader.ReadUInt32();
                            reader.ReadUInt32();
                            reader.ReadUInt32();
                        }
                        if (bitArray.Get(40))
                        {
                            reader.ReadUInt32();
                            reader.ReadUInt32();
                            reader.ReadUInt32();
                            reader.ReadUInt32();
                            reader.ReadUInt32();
                            reader.ReadUInt32();
                        }
                        if (bitArray.Get(41))
                        {
                            reader.ReadUInt32();
                            reader.ReadUInt32();
                            reader.ReadUInt32();
                        }
                        if (bitArray.Get(42))
                        {
                            reader.ReadByte();
                        }
                        if (bitArray.Get(43))
                        {
                            reader.ReadByte();
                            reader.ReadByte();
                            reader.ReadInt16(); // skip 2
                            reader.ReadUInt32();
                            reader.ReadUInt32();
                            reader.ReadByte();
                            reader.ReadByte();
                            reader.ReadByte();
                            reader.ReadByte(); // skip 1
                            reader.ReadInt16();
                            reader.ReadInt16(); // skip 2
                            for (int i = 0; i < 32; ++i)
                            {
                                reader.ReadByte();
                                reader.ReadByte();
                                reader.ReadByte();
                                reader.ReadByte();
                                reader.ReadUInt32();
                            }
                            reader.ReadUInt32();
                        }
                        if (bitArray.Get(44))
                        {
                            for (int i = 0; i < 13; i++)
                            {
                                reader.ReadByte();
                                reader.ReadByte();
                                reader.ReadInt16(); // skip 2
                                reader.ReadUInt32();
                                reader.ReadUInt32();
                                reader.ReadByte();
                                reader.ReadByte();
                                reader.ReadByte();
                                reader.ReadByte(); // skip 1
                                reader.ReadInt16();
                                reader.ReadInt16(); // skip 2
                                for (int j = 0; j < 32; ++j)
                                {
                                    reader.ReadByte();
                                    reader.ReadByte();
                                    reader.ReadByte();
                                    reader.ReadByte();
                                    reader.ReadUInt32();
                                }
                                reader.ReadUInt32();
                            }
                        }
                        if (bitArray.Get(45))
                        {
                            Name = Encoding.ASCII.GetString(reader.ReadBytes(16));
                            Name = Name.TrimEnd(' ', '\0');

                            Console.WriteLine("{0} identified as {1}", IP, Name);

                            ClientIdentified = true;

                            if (OnClientIdentified != null)
                                OnClientIdentified(this);
                        }
                        if (bitArray.Get(46))
                        {
                            for (int i = 0; i < 11; i++)
                            {
                                reader.ReadUInt32();
                            }
                        }
                        if (bitArray.Get(47))
                        {
                            reader.ReadUInt32();
                        }
                    }
                }
                #endregion

                server.BeginWrite(serverSendBuffer, 0, numBytes, ServerDataSentCallback, null);
            }
            catch (Exception ex)
            {
                Disconnect();
                logError(ex);
            }
        }

        private void ServerDataSentCallback(IAsyncResult ar)
        {
            try
            {
                server.EndWrite(ar);

                if (!ClientIdentified)
                    client.BeginRead(packageIntReceiveBuffer, 0, IntSize, ClientIDReceivedCallback, null);
                else
                {
                    clientReceiveBuffer = new byte[BUFFERSIZE];
                    serverSendBuffer = new byte[BUFFERSIZE];

                    client.BeginRead(clientReceiveBuffer, 0, BUFFERSIZE, ClientReceivedNonparseCallback, null);
                }
            }
            catch (Exception ex)
            {
                Disconnect();
                logError(ex);
            }
        }

        private void ClientReceivedNonparseCallback(IAsyncResult ar)
        {
            try
            {
                int numBytes = client.EndRead(ar);

                if (numBytes == 0)
                {
                    throw new Exception("No bytes have been received: disconnect.");
                }
                else
                {
                    Array.Copy(clientReceiveBuffer, 0, serverSendBuffer, 0, numBytes);
                    server.BeginWrite(serverSendBuffer, 0, numBytes, ServerDataSentCallback, null);
                }
            }
            catch (Exception ex)
            {
                Disconnect();
                logError(ex);
            }
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
                Disconnect();
                logError(ex);
            }
        }

        private void ClientWriteCallback(IAsyncResult ar)
        {
            try
            {
                client.EndWrite(ar);
                server.BeginRead(serverReceiveBuffer, 0, BUFFERSIZE, new AsyncCallback(ServerReceivedCallback), null);
            }
            catch (Exception ex)
            {
                Disconnect();
                logError(ex);
            }
        }

        public void Disconnect()
        {
            try
            {
                if (Connected)
                {
                    client.Close();
                    server.Close();

                    if (OnClientDisconnected != null)
                        OnClientDisconnected(this);

                    Connected = false;
                    Console.WriteLine("{0} has disconnected.", IP);
                }
            }
            catch (Exception ex)
            {
                logError(ex);
            }
        }

        private void logError(Exception ex, [CallerMemberName] string memberName = "")
        {
            if (!(ex is IOException) && !(ex is ObjectDisposedException))
            {
                Console.WriteLine("An error occured: {0} - {1}", ex.GetType().ToString(), ex.Message);
                Console.WriteLine("In: {0}", memberName);
            }
        }
    }
}

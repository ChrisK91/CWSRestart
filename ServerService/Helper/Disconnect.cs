using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ServerService.Helper;

namespace ServerService.Helper
{
    /// <summary>
    /// This is a generic class for disconnecting TCP connections.
    /// This class can be used to
    /// 1. Get a list of all connections.
    /// 2. Cloas a connection
    /// </summary>
    public static class DisconnectWrapper
    {
        /// <summary>
        /// Enumeration of connection states
        /// </summary>
        private enum ConnectionState
        {
            All = 0,
            Closed = 1,
            Listen = 2,
            SynSent = 3,
            SynRcvd = 4,
            Established = 5,
            FinWait1 = 6,
            FinWait2 = 7,
            CloseWait = 8,
            Closing = 9,
            LastAck = 10,
            TimeWait = 11,
            DeleteTCB = 12
        }

        /// <summary>
        /// Connection information
        /// </summary>
        private struct ConnectionInfo
        {
            public int dwState;
            public int dwLocalAddr;
            public int dwLocalPort;
            public int dwRemoteAddr;
            public int dwRemotePort;
        }

        /// <summary>
        /// Close all connection to the remote IP
        /// </summary>
        /// <param name="IP">IP to close</param>
        /// <param name="remotePort">The remote port, wildcard 0</param>
        public static int CloseRemoteIP(string IP, int remotePort = 0)
        {
            int ret = -1;

            ConnectionInfo[] rows = getTcpTable();
            for (int i = 0; i < rows.Length; i++)
            {
                if (rows[i].dwRemoteAddr == IPStringToInt(IP)
                    && (remotePort == 0 || (NativeMethods.ntohs(rows[i].dwRemotePort) == remotePort))
                    )
                {
                    rows[i].dwState = (int)ConnectionState.DeleteTCB;
                    IntPtr ptr = GetPtrToNewObject(rows[i]);
                    ret = NativeMethods.SetTcpEntry(ptr);

                    return ret;
                }
            }

            return ret;
        }

        /// <summary>
        /// The function that fills the ConnectionInfo array with connectioninfos
        /// </summary>
        /// <returns>ConnectionInfo</returns>
        private static ConnectionInfo[] getTcpTable()
        {
            IntPtr buffer = IntPtr.Zero; bool allocated = false;
            try
            {
                int iBytes = 0;
                NativeMethods.GetTcpTable(IntPtr.Zero, ref iBytes, false);
                buffer = Marshal.AllocCoTaskMem(iBytes);

                allocated = true;
                NativeMethods.GetTcpTable(buffer, ref iBytes, false);
                int structCount = Marshal.ReadInt32(buffer);
                IntPtr buffSubPointer = buffer;

                if (IntPtr.Size == 8)
                {
                    buffSubPointer = (IntPtr)((long)buffer + 4);
                }
                else
                {
                    buffSubPointer = (IntPtr)((int)buffer + 4);
                }

                ConnectionInfo[] tcpRows = new ConnectionInfo[structCount];

                ConnectionInfo tmp = new ConnectionInfo();
                int sizeOfTCPROW = Marshal.SizeOf(tmp);

                for (int i = 0; i < structCount; i++)
                {
                    tcpRows[i] = (ConnectionInfo)Marshal.PtrToStructure(buffSubPointer, typeof(ConnectionInfo));

                    if (IntPtr.Size == 8)
                    {
                        buffSubPointer = (IntPtr)((long)buffSubPointer + sizeOfTCPROW);
                    }
                    else
                    {
                        buffSubPointer = (IntPtr)((int)buffSubPointer + sizeOfTCPROW);
                    }
                }

                return tcpRows;
            }
            catch (Exception ex)
            {
                Logging.OnLogMessage("Could not get connected players! [" + ex.GetType().ToString() + "," + ex.Message + "]", MessageType.Error);
            }
            finally
            {
                if (allocated) Marshal.FreeCoTaskMem(buffer);
            }

            return new ConnectionInfo[0];
        }

        /// <summary>
        /// Object pointer
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>Pointer</returns>
        private static IntPtr GetPtrToNewObject(object obj)
        {
            IntPtr ptr = Marshal.AllocCoTaskMem(Marshal.SizeOf(obj));
            Marshal.StructureToPtr(obj, ptr, false);
            return ptr;
        }

        /// <summary>
        /// IP to Int
        /// </summary>
        /// <param name="IP">IP Address</param>
        /// <returns>Integer</returns>
        private static int IPStringToInt(string IP)
        {
            if (IP.IndexOf(".") < 0) throw new ArgumentException("Invalid IP address", "IP");
            string[] addr = IP.Split('.');
            if (addr.Length != 4) throw new ArgumentException("Invalid IP address", "IP");
            byte[] bytes = new byte[] { byte.Parse(addr[0]), byte.Parse(addr[1]), byte.Parse(addr[2]), byte.Parse(addr[3]) };
            return BitConverter.ToInt32(bytes, 0);
        }
    }
}

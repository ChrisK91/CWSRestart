using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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
        public enum ConnectionState
        {
            All = 0,
            Closed = 1,
            Listen = 2,
            Syn_Sent = 3,
            Syn_Rcvd = 4,
            Established = 5,
            Fin_Wait1 = 6,
            Fin_Wait2 = 7,
            Close_Wait = 8,
            Closing = 9,
            Last_Ack = 10,
            Time_Wait = 11,
            Delete_TCB = 12
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
        /// Win 32 API for get all connection
        /// </summary>
        /// <param name="pTcpTable">Pointer to TCP table</param>
        /// <param name="pdwSize">Size</param>
        /// <param name="bOrder">Order</param>
        /// <returns>Number</returns>
        [DllImport("iphlpapi.dll")]
        private static extern int GetTcpTable(IntPtr pTcpTable, ref int pdwSize, bool bOrder);

        /// <summary>
        /// Set the connection state
        /// </summary>
        /// <param name="pTcprow">Pointer to TCP table row</param>
        /// <returns>Status</returns>
        [DllImport("iphlpapi.dll")]
        private static extern int SetTcpEntry(IntPtr pTcprow);

        /// <summary>
        /// Convert 16-bit value from network to host byte order
        /// </summary>
        /// <param name="netshort">network host</param>
        /// <returns>host byte order</returns>
        [DllImport("wsock32.dll")]
        private static extern int ntohs(int netshort);

        /// <summary>
        /// //Convert 16-bit value back again
        /// </summary>
        /// <param name="netshort"></param>
        /// <returns></returns>
        [DllImport("wsock32.dll")]
        private static extern int htons(int netshort);

        /// <summary>
        /// Close all connection to the remote IP
        /// </summary>
        /// <param name="IP">IP to close</param>
        public static int CloseRemoteIP(string IP)
        {
            int ret = -1;

            ConnectionInfo[] rows = getTcpTable();
            for (int i = 0; i < rows.Length; i++)
            {
                if (rows[i].dwRemoteAddr == IPStringToInt(IP))
                {
                    rows[i].dwState = (int)ConnectionState.Delete_TCB;
                    IntPtr ptr = GetPtrToNewObject(rows[i]);
                    ret = SetTcpEntry(ptr);
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
                GetTcpTable(IntPtr.Zero, ref iBytes, false);
                buffer = Marshal.AllocCoTaskMem(iBytes);

                allocated = true;
                GetTcpTable(buffer, ref iBytes, false);
                int structCount = Marshal.ReadInt32(buffer);
                IntPtr buffSubPointer = buffer;
                buffSubPointer = (IntPtr)((int)buffer + 4);
                ConnectionInfo[] tcpRows = new ConnectionInfo[structCount];
                
                ConnectionInfo tmp = new ConnectionInfo();
                int sizeOfTCPROW = Marshal.SizeOf(tmp);
                
                for (int i = 0; i < structCount; i++)
                {
                    tcpRows[i] = (ConnectionInfo)Marshal.PtrToStructure(buffSubPointer, typeof(ConnectionInfo));
                    buffSubPointer = (IntPtr)((int)buffSubPointer + sizeOfTCPROW);
                }

                return tcpRows;
            }
            catch (Exception ex)
            {
                throw new Exception("getTcpTable failed! [" + ex.GetType().ToString() + "," + ex.Message + "]");
            }
            finally
            {
                if (allocated) Marshal.FreeCoTaskMem(buffer);
            }
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
            if (IP.IndexOf(".") < 0) throw new Exception("Invalid IP address");
            string[] addr = IP.Split('.');
            if (addr.Length != 4) throw new Exception("Invalid IP address");
            byte[] bytes = new byte[] { byte.Parse(addr[0]), byte.Parse(addr[1]), byte.Parse(addr[2]), byte.Parse(addr[3]) };
            return BitConverter.ToInt32(bytes, 0);
        }
    }
}

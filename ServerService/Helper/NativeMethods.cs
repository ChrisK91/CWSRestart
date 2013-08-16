using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ServerService.Helper
{
    internal static class NativeMethods
    {
        /// <summary>
        /// Win 32 API for get all connection
        /// </summary>
        /// <param name="pTcpTable">Pointer to TCP table</param>
        /// <param name="pdwSize">Size</param>
        /// <param name="bOrder">Order</param>
        /// <returns>Number</returns>
        [DllImport("iphlpapi.dll")]
        internal static extern int GetTcpTable(IntPtr pTcpTable, ref int pdwSize, bool bOrder);

        /// <summary>
        /// Set the connection state
        /// </summary>
        /// <param name="pTcprow">Pointer to TCP table row</param>
        /// <returns>Status</returns>
        [DllImport("iphlpapi.dll")]
        internal static extern int SetTcpEntry(IntPtr pTcprow);

        /// <summary>
        /// Convert 16-bit value from network to host byte order
        /// </summary>
        /// <param name="netshort">network host</param>
        /// <returns>host byte order</returns>
        [DllImport("wsock32.dll")]
        internal static extern int ntohs(int netshort);

        /// <summary>
        /// //Convert 16-bit value back again
        /// </summary>
        /// <param name="netshort"></param>
        /// <returns></returns>
        //[DllImport("wsock32.dll")]
        //internal static extern int htons(int netshort);

        /// <summary>
        /// Sets the foreground window
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        internal static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool OpenProcessToken(IntPtr ProcessHandle, UInt32 DesiredAccess, out IntPtr TokenHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CloseHandle(IntPtr hObject);

        public enum TOKEN_INFORMATION_CLASS
        {
            TokenUser = 1,
            TokenGroups,
            TokenPrivileges,
            TokenOwner,
            TokenPrimaryGroup,
            TokenDefaultDacl,
            TokenSource,
            TokenType,
            TokenImpersonationLevel,
            TokenStatistics,
            TokenRestrictedSids,
            TokenSessionId,
            TokenGroupsAndPrivileges,
            TokenSessionReference,
            TokenSandBoxInert,
            TokenAuditPolicy,
            TokenOrigin,
            TokenElevationType,
            TokenLinkedToken,
            TokenElevation,
            TokenHasRestrictions,
            TokenAccessInformation,
            TokenVirtualizationAllowed,
            TokenVirtualizationEnabled,
            TokenIntegrityLevel,
            TokenUIAccess,
            TokenMandatoryPolicy,
            TokenLogonSid,
            MaxTokenInfoClass
        }

        public enum TOKEN_ELEVATION_TYPE
        {
            TokenElevationTypeDefault = 1,
            TokenElevationTypeFull,
            TokenElevationTypeLimited
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern bool GetTokenInformation(IntPtr TokenHandle, TOKEN_INFORMATION_CLASS TokenInformationClass, IntPtr TokenInformation, uint TokenInformationLength, out uint ReturnLength);

    }
}

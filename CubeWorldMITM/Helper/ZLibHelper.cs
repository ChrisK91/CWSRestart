using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ionic.Zlib;

namespace CubeWorldMITM.Helper
{
    /// <summary>
    /// Contains helper method to hanlde compressed data
    /// </summary>
    internal static class ZLibHelper
    {
        /// <summary>
        /// Uncompresses a zlib compressed byte array
        /// </summary>
        /// <param name="data">The compressed data</param>
        /// <returns>The uncompressed data</returns>
        public static byte[] UncompressBytes(byte[] data)
        {
            byte[] compressed = new byte[data.Length - 2];

            Array.Copy(data, 2, compressed, 0, compressed.Length);
            return DeflateStream.UncompressBuffer(compressed);
        }
    }
}

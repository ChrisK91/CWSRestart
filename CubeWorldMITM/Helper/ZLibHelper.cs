using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ionic.Zlib;

namespace CubeWorldMITM.Helper
{
    internal static class ZLibHelper
    {
        public static byte[] UncompressBytes(byte[] data)
        {
            byte[] compressed = new byte[data.Length - 2];

            Array.Copy(data, 2, compressed, 0, compressed.Length);
            return DeflateStream.UncompressBuffer(compressed);
        }
    }
}

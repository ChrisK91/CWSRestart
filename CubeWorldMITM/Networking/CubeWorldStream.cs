using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeWorldMITM.Networking
{
    class CubeWorldStream
    {
        private Stream stream;

        public CubeWorldStream(Stream s)
        {
            stream = s;
        }

        public byte ReadByte()
        {
            int b = stream.ReadByte();
            return Convert.ToByte(b);  
        }

        public void WriteByte(byte b)
        {
            stream.WriteByte(b);
        }
    }
}

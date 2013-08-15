using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CubeWorldMITM.ServerConfigurators
{
    internal class StandardConfigurator : IConfigurator
    {
        public String MD5
        {
            get
            {
                return "9C69B731CF197236CE800B44B2ABE497";
            }
        }

        public String Name
        {
            get
            {
                return "Standard server.exe patcher";
            }
        }

        private const int offset = 0x27C10;
        private const int desiredPort = 12346;

        public string PrepareFile(string FilePath)
        {
            string tmpDir = Path.Combine(Directory.GetParent(FilePath).ToString());
            String targetFile = Path.Combine(tmpDir, "ServerModified.exe");

            if (!Directory.Exists(tmpDir))
                Directory.CreateDirectory(tmpDir);

            File.Copy(FilePath, targetFile, true);

            using (BinaryWriter bw = new BinaryWriter(File.Open(targetFile, FileMode.Open)))
            {
                bw.Seek(offset, SeekOrigin.Begin);
                bw.Write(desiredPort);
            }

            return targetFile;
        }
    }
}

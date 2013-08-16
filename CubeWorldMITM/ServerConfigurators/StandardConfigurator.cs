using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CubeWorldMITM.ServerConfigurators
{
    /// <summary>
    /// A configurator that patches the port of the server.exe to 12346
    /// </summary>
    internal class StandardConfigurator : IConfigurator
    {
        /// <summary>
        /// The MD5 of the server.exe that comes with CubeWorld
        /// </summary>
        public String MD5
        {
            get
            {
                return "9C69B731CF197236CE800B44B2ABE497";
            }
        }

        /// <summary>
        /// The name of the configurator
        /// </summary>
        public String Name
        {
            get
            {
                return "Standard server.exe patcher";
            }
        }

        /// <summary>
        /// The offset where the port number is located
        /// </summary>
        private const int offset = 0x27C10;

        /// <summary>
        /// The port to wich the server should be patched
        /// </summary>
        private const int desiredPort = 12346;

        /// <summary>
        /// Patches the server.exe and saves the patched server to ServerModified.exe
        /// </summary>
        /// <param name="file">The location of the server.exe</param>
        /// <returns>The path of the patched server</returns>
        public string PrepareFile(string file)
        {
            string tmpDir = Path.Combine(Directory.GetParent(file).ToString());
            String targetFile = Path.Combine(tmpDir, "ServerModified.exe");

            if (!Directory.Exists(tmpDir))
                Directory.CreateDirectory(tmpDir);

            File.Copy(file, targetFile, true);

            using (BinaryWriter bw = new BinaryWriter(File.Open(targetFile, FileMode.Open)))
            {
                bw.Seek(offset, SeekOrigin.Begin);
                bw.Write(desiredPort);
            }

            return targetFile;
        }
    }
}

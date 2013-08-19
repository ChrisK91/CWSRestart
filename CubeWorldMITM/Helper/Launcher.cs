using CubeWorldMITM.ServerConfigurators;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeWorldMITM.Helper
{
    /// <summary>
    /// Prepares and launches the server
    /// </summary>
    internal static class Launcher
    {
        /// <summary>
        /// Containes the location from which the server was launched
        /// </summary>
        public static string ServerLocation { get; private set; }

        /// <summary>
        /// Contains the name of the started server process
        /// </summary>
        public static string ServerName { get; private set; }

        private static Dictionary<string, List<IConfigurator>> _configurators;

        /// <summary>
        /// Associates server.exe MD5 with the related configurators.
        /// </summary>
        public static Dictionary<string, List<IConfigurator>> Configurators
        {
            get
            {
                if (_configurators == null)
                {
                    Console.WriteLine("Loading configurators");

                    _configurators = new Dictionary<string, List<IConfigurator>>();

                    Type IConfiguratorType = typeof(ServerConfigurators.IConfigurator);

                    //Load embedded configurators
                    IEnumerable<Type> Types = AppDomain.CurrentDomain.GetAssemblies().ToList().SelectMany(a => a.GetTypes()).Where(t => IConfiguratorType.IsAssignableFrom(t) && t.IsClass);

                    foreach (Type t in Types)
                    {
                        ServerConfigurators.IConfigurator configurator = (ServerConfigurators.IConfigurator)Activator.CreateInstance(t);

                        if (!_configurators.ContainsKey(configurator.MD5))
                        {
                            _configurators.Add(configurator.MD5, new List<IConfigurator>());
                        }
                        _configurators[configurator.MD5].Add(configurator);

                        Console.WriteLine("Loaded \"{0}\" for MD5 \"{1}\"", configurator.Name, configurator.MD5);
                    }

                    //TODO: Add plugin loading for external configurators
                }

                return _configurators;
            }
        }

        /// <summary>
        /// Configures and launches the server. See also: ServerLocation and ServerName which are set by this method
        /// </summary>
        /// <param name="path">The original path of the server</param>
        public static void LaunchServerConfigured(string path)
        {
            string originalPath = path;

            byte[] md5;

            using (FileStream file = File.OpenRead(path))
            {
                System.Security.Cryptography.MD5 md5Provider = new System.Security.Cryptography.MD5CryptoServiceProvider();
                md5 = md5Provider.ComputeHash(file);
            }

            string hash = BitConverter.ToString(md5).Replace("-", "");
            Console.WriteLine("MD5: {0}", hash);

            if (Configurators.ContainsKey(hash))
            {
                foreach (IConfigurator c in Configurators[hash])
                {
                    path = c.PrepareFile(path);
                }
            }

            ProcessStartInfo pi = new ProcessStartInfo(path);
            //Sets the working directory to the original path
            pi.WorkingDirectory = Path.GetDirectoryName(originalPath);
            Process p = Process.Start(pi);

            ServerLocation = path;
            ServerName = p.ProcessName;
        }
    }
}

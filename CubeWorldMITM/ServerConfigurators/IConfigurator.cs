using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeWorldMITM.ServerConfigurators
{
    public interface IConfigurator
    {
        /// <summary>
        /// The MD5 of the server.exe, that will be handled by this 
        /// </summary>
        String MD5 { get; }
        /// <summary>
        /// The name of the configurator
        /// </summary>
        String Name { get; }
        /// <summary>
        /// Will be called if the launched server has the same MD5 as specified in MD5
        /// </summary>
        /// <param name="FilePath">The location of the server exectuable</param>
        /// <returns>The path of the server that should be launched</returns>
        string PrepareFile(string FilePath);
    }
}

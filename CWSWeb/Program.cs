using Nancy.Hosting.Self;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWSWeb
{
    class Program
    {
        static void Main(string[] args)
        {
            int port = 8181;

            if (args.Length == 1)
            {
                Int32.TryParse(args[0], out port);
            }

            HostConfiguration config = new HostConfiguration();

            config.UrlReservations.CreateAutomatically = true;
            config.UrlReservations.User = new System.Security.Principal.SecurityIdentifier("S-1-1-0").Translate(typeof(System.Security.Principal.NTAccount)).ToString();

            NancyHost host = new NancyHost(new ApplicationBootstrapper(),config, new Uri(String.Format("http://localhost:{0}", port)));
            Console.WriteLine("Starting server on http://localhost:" + port.ToString());


            host.Start();
            MessageLoop();

            host.Stop();
        }

        private static void MessageLoop()
        {
            string message = "start";
            Helper.Settings.Client = new CWSProtocol.Client("WebServer");

            while (message != "quit")
            {
                switch (message)
                {
                    case "test":
                        if (Helper.Settings.Client.Test())
                            Console.WriteLine("Connection succesful.");
                        else
                            Console.WriteLine("No connection possible.");
                        break;
                }

                message = "";
                Console.WriteLine("Enter \"quit\" to quit");
                Console.WriteLine("Enter \"test\" to test communicating with CWSRestart");
                message = Console.ReadLine().ToLower();
            }
        }
    }
}

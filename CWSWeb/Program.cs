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
            string message = "start";
            Client c = new Client();

            while (message != "quit")
            {
                switch(message)
                {
                    case "start":
                        c.Connect();
                        break;

                    case "stop":
                        c.Stop();
                        break;
                }

                message = "";
                Console.WriteLine("Enter \"quit\" to quit");
                Console.WriteLine("Enter \"start\" to start communicating with CWSRestart");
                Console.WriteLine("Enter \"stop\" to stop communicating with CWSRestart");
                message = Console.ReadLine().ToLower();
            }

            host.Stop();
        }
    }
}

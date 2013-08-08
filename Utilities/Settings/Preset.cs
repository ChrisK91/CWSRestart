using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace Utilities.Settings
{
    public class Preset
    {
        public string Name { get; private set; }

        public string ServerLocation { get; private set; }
        public string ProcessName { get; private set; }

        public int? Port { get; private set; }
        public bool? DoNotRedirectOutput { get; private set; }
        public bool? BypassSendQuit { get; private set; }

        public ReadOnlyCollection<string> AdditionalProcesses { get; private set; }
        public ReadOnlyDictionary<string, bool> Checks { get; private set; }

        public const string LANAccess = "LAN";
        public const string LoopbackAccess = "Loopback";
        public const string InternetAccess = "Internet";

        public Preset(string name = null, string serverLocation = null, string processName = null, int? port = null, bool? doNotRedirectOutput = null, bool? bypassSendQuit = null, IEnumerable<string> additionalProcesses = null, IDictionary<string, bool> checks = null)
        {
            this.Name = name;
            this.ServerLocation = serverLocation;
            this.ProcessName = processName;
            this.Port = port;
            this.DoNotRedirectOutput = doNotRedirectOutput;
            this.BypassSendQuit = bypassSendQuit;
            this.AdditionalProcesses = additionalProcesses != null ? new ReadOnlyCollection<string>(new List<string>(additionalProcesses)) : null;
            this.Checks = checks != null ? new ReadOnlyDictionary<string,bool>(checks) : null;
        }

        public static Preset Load(string filename)
        {
            string name = null;
            string serverLocation = null;
            string processName = null;
            int? port = null;
            bool? doNotRedirectOutput = null;
            bool? bypassSendQuit = null;
            List<string> additionalProcesses = new List<string>();
            Dictionary<string, bool> checks = new Dictionary<string, bool>();

            XmlSchema presetSchema;
            XmlSchemaSet set = new XmlSchemaSet();

            using (Stream resource = Assembly.GetExecutingAssembly().GetManifestResourceStream("Utilities.Embedded.PresetSchema.xsd"))
            {
                presetSchema = XmlSchema.Read(resource, null);
            }

            XDocument preset = XDocument.Load(filename);

            set.Add(presetSchema);
            preset.Validate(set, null);

            XElement presetElement = preset.Descendants("Preset").First();
            XElement serverElement = presetElement.Descendants("Server").FirstOrDefault();

            var checksEntries = from item in preset.Descendants("Checks").Descendants("Check")
                                where item.Attribute("Name") != null && item.Attribute("Enabled") != null
                                select new
                                    {
                                        name = item.Attribute("Name").Value,
                                        enabled = Convert.ToBoolean(item.Attribute("Enabled").Value)
                                    };

            var proccesesEntries = from item in preset.Descendants("AdditionalProcesses").Descendants("Process")
                                   where item.Attribute("Process") != null
                                   select new
                                   {
                                       name = item.Attribute("Process").Value
                                   };

            name = presetElement.Attribute("Name") != null ? presetElement.Attribute("Name").Value : "undefined";

            if (serverElement != null)
            {
                serverLocation = serverElement.Attribute("Location") != null ? serverElement.Attribute("Location").Value : null;
                port = serverElement.Attribute("Port") != null ? (int?)Convert.ToInt32(serverElement.Attribute("Port").Value) : null;
                processName = serverElement.Attribute("Process") != null ? serverElement.Attribute("Process").Value : null;
                doNotRedirectOutput = serverElement.Attribute("DoNotRedirectOutput") != null ? (bool?)Convert.ToBoolean(serverElement.Attribute("DoNotRedirectOutput").Value) : null;
                bypassSendQuit = serverElement.Attribute("BypassSendQuit") != null ? (bool?)Convert.ToBoolean(serverElement.Attribute("BypassSendQuit").Value) : null;
            }

            if (checksEntries.Count() > 0)
                foreach (var entry in checksEntries)
                    checks.Add(entry.name, entry.enabled);


            if (proccesesEntries.Count() > 0)
                foreach (var entry in proccesesEntries)
                    additionalProcesses.Add(entry.name);

            return new Preset()
            {
                Name = name,
                ServerLocation = serverLocation,
                Port = port,
                ProcessName = processName,
                DoNotRedirectOutput = doNotRedirectOutput,
                BypassSendQuit = bypassSendQuit,
                AdditionalProcesses = new ReadOnlyCollection<string>(additionalProcesses),
                Checks = new ReadOnlyDictionary<string, bool>(checks)
            };
        }

        public void Save(string filename)
        {
            XDocument doc = new XDocument();
            XElement root = new XElement("Preset", new XAttribute("Name", Name ?? "undefined"));

            if ((ServerLocation != null) || (ProcessName != null) || (Port != null) || (DoNotRedirectOutput != null) || (BypassSendQuit != null) || (Checks != null && Checks.Count > 0))
            {
                XElement server = new XElement("Server");

                if (ServerLocation != null)
                    server.Add(new XAttribute("Location", ServerLocation));

                if (Port != null)
                    server.Add(new XAttribute("Port", Port));

                if (ProcessName != null)
                    server.Add(new XAttribute("Process", ProcessName));

                if (DoNotRedirectOutput != null)
                    server.Add(new XAttribute("DoNotRedirectOutput", DoNotRedirectOutput));

                if (BypassSendQuit != null)
                    server.Add(new XAttribute("BypassSendQuit", BypassSendQuit));

                if (Checks != null && Checks.Count > 0)
                {
                    XElement checks = new XElement("Checks");

                    foreach (KeyValuePair<string, bool> kvp in Checks)
                    {
                        checks.Add(
                            new XElement("Check",
                                new XAttribute("Name", kvp.Key),
                                new XAttribute("Enabled", kvp.Value))
                            );
                    }
                    server.Add(checks);
                }

                root.Add(server);
            }

            if (AdditionalProcesses != null && AdditionalProcesses.Count > 0)
            {
                XElement additional = new XElement("AdditionalProcesses");

                foreach (string s in AdditionalProcesses)
                    additional.Add(new XElement("Process", new XAttribute("Name", s)));

                root.Add(additional);
            }

            doc.Add(root);
            doc.Save(filename);
        }
    }
}

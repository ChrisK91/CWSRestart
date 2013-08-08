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

        private Preset() { }

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
    }
}

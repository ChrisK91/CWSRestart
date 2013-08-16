using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWSWeb.Helper
{
    public class DataContainer
    {
        public string Key { get; private set; }
        public string Value { get; private set; }

        public DataContainer(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}

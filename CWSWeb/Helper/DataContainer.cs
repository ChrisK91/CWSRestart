using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWSWeb.Helper
{
    public class DataContainer
    {
        public string Key;
        public string Value;

        public DataContainer(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}

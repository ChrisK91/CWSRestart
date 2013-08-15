using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ServerService.Helper
{
    public abstract class AccessListEntry : INotifyPropertyChanged
    {
        public abstract override string ToString();
        public abstract string FriendlyName { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public static bool TryParse(string source, out AccessListEntry entry)
        {
            throw new NotImplementedException();
        }

        public abstract bool Matches(IPAddress target);

        protected void notifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

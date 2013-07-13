using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ServerService
{
    public sealed class AccessControl : INotifyPropertyChanged
    {
        public enum AccessMode
        {
            Whitelist,
            Blacklist
        }

        private static readonly AccessControl instance = new AccessControl();
        public event PropertyChangedEventHandler PropertyChanged;

        public static AccessControl Instance
        {
            get
            {
                return instance;
            }
        }

        private ObservableCollection<IPAddress> accessList = new ObservableCollection<IPAddress>();
        public ObservableCollection<IPAddress> AccessList
        {
            get
            {
                return accessList;
            }
            set
            {
                if (accessList != value)
                {
                    accessList = value;
                    notifyPropertyChanged();
                }
            }
        }

        private AccessMode mode = AccessMode.Whitelist;
        public AccessMode Mode
        {
            get
            {
                return mode;
            }
            set
            {
                if (mode != value)
                {
                    mode = value;
                    notifyPropertyChanged();
                }
            }
        }

        private AccessControl()
        {
        }

        private void notifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

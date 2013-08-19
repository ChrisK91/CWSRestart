using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ServerService.Access.Entries
{
    /// <summary>
    /// A base class for access list entries. Derived classes should implement a static TryParse method that creates an entry from a given string.
    /// </summary>
    public abstract class AccessListEntry : INotifyPropertyChanged
    {
        /// <summary>
        /// Casts the list entry to a string
        /// </summary>
        /// <returns>A readble representation of the entry</returns>
        public abstract override string ToString();

        /// <summary>
        /// A friendly name (for instance with the playername)
        /// </summary>
        public abstract string FriendlyName { get; }

        /// <summary>
        /// Notifies parents of changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Tries to create
        /// </summary>
        /// <param name="source"></param>
        /// <param name="entry"></param>
        /// <returns></returns>
        //public static bool TryParse(string source, out AccessListEntry entry)
        //{
        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// Checks if a given IP is matched by the entry
        /// </summary>
        /// <param name="target">The IP to check</param>
        /// <returns>True if the entry matches the IP, otherwise false</returns>
        public abstract bool Matches(IPAddress target);

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

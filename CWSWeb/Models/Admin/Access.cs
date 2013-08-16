using ServerService.Access;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWSWeb.Models.Admin
{
    public class Access
    {
        public Collection<string> ConnectedPlayers { get; private set; }
        public Collection<string> ListEntries { get; private set; }
        public AccessMode AccessMode { get; private set; }

        public Access(Collection<string> ConnectedPlayers, Collection<string> ListEntries, AccessMode AccessMode)
        {
            this.ConnectedPlayers = ConnectedPlayers;
            this.ListEntries = ListEntries;
            this.AccessMode = AccessMode;
        }
    }
}

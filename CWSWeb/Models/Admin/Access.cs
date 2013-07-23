using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWSWeb.Models.Admin
{
    public class Access
    {
        public List<string> ConnectedPlayers { get; private set; }
        public List<string> ListEntries { get; private set; }
        public ServerService.AccessControl.AccessMode AccessMode { get; private set; }

        public Access(List<string> ConnectedPlayers, List<string> ListEntries, ServerService.AccessControl.AccessMode AccessMode )
        {
            this.ConnectedPlayers = ConnectedPlayers;
            this.ListEntries = ListEntries;
            this.AccessMode = AccessMode;
        }
    }
}

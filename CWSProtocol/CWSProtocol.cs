using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWSProtocol
{
    public static class Commands
    {
        public enum Action
        {
            Get,
            Post
        }

        public enum Target
        {
            CurrentStatistics,
            IsAlive
        }
    }
}

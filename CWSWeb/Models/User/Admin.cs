using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWSWeb.Models.User
{
    public class Admin
    {
        public string Username { get; private set; }

        public Admin(string name)
        {
            Username = name;
        }
    }
}

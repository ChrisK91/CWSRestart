using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy.Security;
using Nancy.Authentication.Forms;
using Nancy;
using System.Security.Cryptography;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;

namespace CWSWeb.Helper
{
    internal class Users : IUserMapper
    {
        /// <summary>
        /// Contains the users in the following formate
        /// Name - Password - Salt - GUID
        /// </summary>
        private static List<Tuple<string, string, string, Guid>> users = new List<Tuple<string, string, string, Guid>>();

        static Users()
        {
        }

        public static bool AddUser(string name, string password)
        {
            if (users.Count(u => u.Item1 == name) == 0)
            {
                string salt = HashProvider.GenerateSalt(16);
                string hash = HashProvider.GetHash(password, salt);

                users.Add(new Tuple<string, string, string, Guid>(name, hash, salt, Guid.NewGuid()));
                return true;
            }

            return false;
        }

        public static List<string> GetUserNames()
        {
            List<string> ret = new List<string>();

            foreach (Tuple<string, string, string, Guid> item in users)
            {
                ret.Add(item.Item1);
            }

            return ret;
        }

        public static bool RemoveUser(string name)
        {
            Tuple<string, string, string, Guid> userRow = users.Where(u => String.Compare(u.Item1, name, true) == 0).FirstOrDefault();

            if (userRow != null)
            {
                users.Remove(userRow);
                return true;
            }

            return false;
        }

        public IUserIdentity GetUserFromIdentifier(Guid identifier, NancyContext ctx)
        {
            Tuple<string, string, string, Guid> userRow = users.Where(u => u.Item4 == identifier).FirstOrDefault();

            if (userRow == null)
                return null;
            return new Administrator()
            {
                UserName = userRow.Item1
            };
        }

        public static Guid? ValidateUser(string username, string password)
        {
            Tuple<string, string, string, Guid> userRow = users.Where(u => String.Compare(u.Item1, username, true) == 0).FirstOrDefault();

            if (userRow != null)
            {
                if (userRow.Item2 == HashProvider.GetHash(password, userRow.Item3))
                    return userRow.Item4;
            }

            return null;
        }

        public static bool TryLoadUsersFromFile(string filename)
        {
            if (File.Exists(filename))
            {
                FileStream fs = null;
                try
                {
                    fs = File.Open(filename, FileMode.Open, FileAccess.Read);
                    BinaryFormatter bf = new BinaryFormatter();
                    users = (List<Tuple<string, string, string, Guid>>)bf.Deserialize(fs);
                }
                catch (Exception)
                {
                    if (Debugger.IsAttached)
                        Debugger.Break();

                    return false;
                }
                finally
                {
                    if (fs != null)
                        fs.Close();
                }
            }
            return false;
        }

        public static bool TrySaveUsersToFile(string filename)
        {
            if (File.Exists(filename))
                File.Delete(filename);

            FileStream fs = null;

            try
            {
                fs = File.Open(filename, FileMode.Create, FileAccess.ReadWrite);
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, users);
                return true;
            }
            catch (Exception)
            {
                if (Debugger.IsAttached)
                    Debugger.Break();

                return false;
            }
            finally
            {
                fs.Close();
            }

        }
    }
}

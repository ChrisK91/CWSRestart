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

namespace CWSWeb.Helper.Users
{
    public class Authentication : IUserMapper
    {
        /// <summary>
        /// Contains the users in the following formate
        /// Name - Password - Salt - GUID - Claims (seperated by | )
        /// </summary>
        private static List<Tuple<string, string, string, Guid, string>> users = new List<Tuple<string, string, string, Guid, string>>();
        public const string ADMINISTRATOR = "administration";
        public const string PREMIUM = "premium";


        public static bool AddUser(string name, string password, string claims)
        {
            if (users.Count(u => u.Item1 == name) == 0)
            {
                string salt = HashProvider.GenerateSalt(16);
                string hash = HashProvider.GetHash(password, salt);

                users.Add(new Tuple<string, string, string, Guid, string>(name, hash, salt, Guid.NewGuid(), claims));
                return true;
            }

            return false;
        }

        public static List<Tuple<string, string>> GetUserNames()
        {
            List<Tuple<string, string>> ret = new List<Tuple<string, string>>();

            foreach (Tuple<string, string, string, Guid, string> item in users)
            {
                ret.Add(new Tuple<string, string>(item.Item1, item.Item5));
            }

            return ret;
        }

        public static bool RemoveUser(string name)
        {
            Tuple<string, string, string, Guid, string> userRow = users.Where(u => String.Compare(u.Item1, name, true) == 0).FirstOrDefault();

            if (userRow != null)
            {
                users.Remove(userRow);
                return true;
            }

            return false;
        }

        public IUserIdentity GetUserFromIdentifier(Guid identifier, NancyContext ctx)
        {
            Tuple<string, string, string, Guid, string> userRow = users.Where(u => u.Item4 == identifier).FirstOrDefault();

            if (userRow == null)
                return null;
            return new User()
            {
                UserName = userRow.Item1,
                UserId = userRow.Item4,
                Claims = userRow.Item5.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries)
            };
        }

        public static Guid? ValidateUser(string username, string password)
        {
            Tuple<string, string, string, Guid, string> userRow = users.Where(u => String.Compare(u.Item1, username, true) == 0).FirstOrDefault();

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
                    users = (List<Tuple<string, string, string, Guid, string>>)bf.Deserialize(fs);
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

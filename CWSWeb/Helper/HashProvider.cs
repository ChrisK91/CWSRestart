using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CWSWeb.Helper
{
    public static class HashProvider
    {
        public static string GetHash(string secret, string salt)
        {
            SHA256 sha = SHA256Managed.Create();
            return Convert.ToBase64String(sha.ComputeHash(saltString(secret, salt)));
        }

        private static byte[] saltString(string secret, string salt)
        {
            byte[] secretBytes = Encoding.UTF8.GetBytes(secret);
            byte[] saltBytes = Encoding.UTF8.GetBytes(salt);

            byte[] ret = new byte[secretBytes.Length + saltBytes.Length];

            for (int i = 0; i < secretBytes.Length; i++)
                ret[i] = secretBytes[i];
            for (int i = 0; i < saltBytes.Length; i++)
                ret[i + secretBytes.Length] = saltBytes[i];

            return ret;
        }

        public static string GenerateSalt(int size)
        {
            RNGCryptoServiceProvider generator = new RNGCryptoServiceProvider();
            byte[] buffer = new byte[size];
            generator.GetBytes(buffer);

            return Convert.ToBase64String(buffer);
        }
    }
}

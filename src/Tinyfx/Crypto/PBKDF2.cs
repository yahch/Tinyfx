using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Tinyfx.Crypto
{
    public class PBKDF2
    {
        public static string Encrypt(string str)
        {
            byte[] salt = new byte[] { 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x69, 0x48, 0x56, 0x30, 0x6F, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x28, 0x4E, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30 };
            var rfc = new Rfc2898DeriveBytes(str, salt, 100);
            byte[] bout = rfc.GetBytes(32);
            return Convert.ToBase64String(bout);
        }
    }
}

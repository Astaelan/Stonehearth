using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace StonehearthCommon
{
    public static class PasswordHasher
    {
        public static string HashStrings(string pString1, string pString2)
        {
            byte[] string1Bytes = Encoding.ASCII.GetBytes(pString1);
            byte[] string2Bytes = Encoding.ASCII.GetBytes(pString2);
            MemoryStream stream = new MemoryStream();
            stream.Write(string1Bytes, 0, string1Bytes.Length);
            stream.Write(string2Bytes, 0, string2Bytes.Length);
            return BitConverter.ToString(new SHA512Managed().ComputeHash(stream.ToArray())).Replace("-", "").ToUpper();
        }
    }
}
